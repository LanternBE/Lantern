using System.Net;
using BedrockProtocol;
using BedrockProtocol.Types;
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
        
        ConfigManager = new ConfigManager();
        IpEndPoint = ipEndPoint;
        Debug = ConfigManager.Settings.DebugMode;
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
        var outerPacket = BedrockPacketFactory.CreateFromBuffer(encapsulatedPacket.Buffer);
        if (outerPacket is null) {
            Logger.LogWarn($"Unknown or unsupported BedrockPacket: ({encapsulatedPacket.Buffer[0]}) from ({clientSession.RemoteEndPoint})");
            return;
        }

        BedrockPacket? maybeBedrockPacket;
        byte[] payloadBuffer;

        if (outerPacket is GamePacket gamePacket) {
            if (gamePacket.Payload is null || gamePacket.Payload.Length == 0) {
                Logger.LogWarn($"Received an empty Payload from GamePacket ({clientSession.RemoteEndPoint})");
                return;
            }

            payloadBuffer = gamePacket.Payload;
            maybeBedrockPacket = BedrockPacketFactory.CreateFromBuffer(payloadBuffer);
        } else {
            payloadBuffer = encapsulatedPacket.Buffer;
            maybeBedrockPacket = outerPacket;
        }

        if (maybeBedrockPacket is null) {
            Logger.LogWarn($"Unknown or unsupported BedrockPacket: ({payloadBuffer[0]}) from ({clientSession.RemoteEndPoint})");
            return;
        }
        var bedrockPacket = maybeBedrockPacket;

        var packetType = bedrockPacket.GetType();
        var handler = BedrockHandler.CreateHandler(packetType);
        if (handler is null) {
            if (bedrockPacket is RawBedrockPacket rawPacket) {
                var packetId = (int)rawPacket.PacketType;
                if (PacketRegistry.TryGetName(packetId, out var packetName))
                    Logger.LogDebug($"Unsupported Bedrock packet '{packetName}' ({packetId}) from ({clientSession.RemoteEndPoint})");
                else
                    Logger.LogDebug($"Unsupported Bedrock packet ({packetId}) from ({clientSession.RemoteEndPoint})");
            }
            return;
        }

        handler.Initialize(Server, Server.Socket, clientSession.RemoteEndPoint, payloadBuffer, bedrockPacket);
        var handled = await handler.HandleAsync();
        if (!handled) {
            var packetName = packetType.Name;
            var remoteEndPoint = clientSession.RemoteEndPoint?.ToString() ?? "unknown-endpoint";
            Logger.LogDebug($"Packet {packetName} from {remoteEndPoint} was not fully handled.");
        }
    }
}
