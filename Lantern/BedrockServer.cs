using System.Net;
using BedrockProtocol;
using Lantern.Handling;
using Lantern.Utils;
using RakSharp;
using RakSharp.Protocol.Online;
using RakSharp.Utils;
using RakSharp.Utils.Sessions;

namespace Lantern;

public class BedrockServer {

    public BedrockHandler BedrockHandler { get; set; } = new();
    public ConfigManager ConfigManager { get; set; }
    public IPEndPoint IpEndPoint { get; set; }
    public Server Server { get; set; }
    
    public bool Debug { get; set; }
    
    public BedrockServer() {
        
        ConfigManager = new ConfigManager();
        IpEndPoint = new IPEndPoint(IPAddress.Parse(ConfigManager.Settings.ServerIp), ConfigManager.Settings.Port);
        Debug = ConfigManager.Settings.DebugMode;
        Server = new Server(IpEndPoint);

        Server.OnGamePacketReceived += HandleGamePacket;
        BedrockHandler.InitializeDefaultHandlers();
    }

    public BedrockServer(IPEndPoint ipEndPoint) {
        
        IpEndPoint = ipEndPoint;
        Server = new Server(ipEndPoint);
        
        Server.OnGamePacketReceived += HandleGamePacket;
        BedrockHandler.InitializeDefaultHandlers();
    }

    public void Start() {
        
        Logger.LogInfo($"Starting RakNet server on {IpEndPoint}, DEBUG: {Debug}");
        _ = Task.Run(async () => {
            await Server.Start();
        });
    }

    public void Stop() {
        
        _ = Task.Run(async () => {
            await Server.Stop();
        });
    }

    public async Task StartAsync() {
        
        Logger.LogInfo($"Starting RakNet server on {IpEndPoint}, DEBUG: {Debug}");
        await Server.Start();
    }

    public async Task StopAsync() {
        await Server.Stop();
    }

    private async void HandleGamePacket(ClientSession clientSession, EncapsulatedPacket encapsulatedPacket) {
        
        var packet = BedrockPacketFactory.CreateFromBuffer(encapsulatedPacket.Buffer);
        if (packet is not GamePacket gamePacket) {
            Logger.LogWarn($"Unknown or unsupported BedrockPacket: ({encapsulatedPacket.Buffer[0]}) from ({clientSession.RemoteEndPoint})");
            return;
        }

        if (gamePacket.Payload is null || gamePacket.Payload.Length == 0) {
            Logger.LogWarn($"Received an empty Payload from GamePacket ({clientSession.RemoteEndPoint})");
            return;
        }

        var bedrockPacket = BedrockPacketFactory.CreateFromBuffer(gamePacket.Payload);
        if (bedrockPacket is null) {
            Logger.LogWarn($"Unknown or unsupported BedrockPacket: ({encapsulatedPacket.Buffer[0]}) from ({clientSession.RemoteEndPoint})");
            return;
        }
        
        var packetType = bedrockPacket.GetType();
        var handlerType = BedrockHandler.GetHandlerType(packetType);

        if (handlerType is null)
            return;

        var handler = BedrockHandler.CreateHandler(handlerType);
        var initializeMethod = handlerType.GetMethod("Initialize");

        if (initializeMethod == null)
            return;

        initializeMethod.Invoke(handler, [Server, Server.Socket, clientSession.RemoteEndPoint, gamePacket.Payload, bedrockPacket]);
        var handleMethod = handlerType.GetMethod("HandleAsync");

        if (handleMethod == null)
            return;

        var result = handleMethod.Invoke(handler, null);
        if (result is Task<bool> task) {
            await task;
        }
    }
}