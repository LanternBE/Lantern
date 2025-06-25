using System.Net;
using Lantern.Utils;
using RakSharp;
using RakSharp.Protocol.Online;
using RakSharp.Utils;
using RakSharp.Utils.Sessions;

namespace Lantern;

public class BedrockServer {
    
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
    }

    public BedrockServer(IPEndPoint ipEndPoint) {
        
        IpEndPoint = ipEndPoint;
        Server = new Server(ipEndPoint);
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

    public async Task StopStop() {
        await Server.Stop();
    }

    private void HandleGamePacket(ClientSession clientSession, EncapsulatedPacket encapsulatedPacket) {
        Logger.LogInfo($"Received game packet: {encapsulatedPacket.Buffer[0]}");
    }
}