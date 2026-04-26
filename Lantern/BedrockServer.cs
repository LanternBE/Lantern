using System.Net;
using System.Net.Sockets;
using BedrockProtocol;
using BedrockProtocol.Types;
using Lantern.Handling;
using Lantern.Utils;
using RakSharp;
using RakSharp.Packet;
using RakSharp.Protocol;
using RakSharp.Protocol.Online;
using RakSharp.Utils;
using RakSharp.Utils.Sessions;

namespace Lantern;

public class BedrockServer {

    // Sized to comfortably fit Bedrock/RakNet traffic while avoiding oversized per-loop allocations.
    private const int MaxUdpPacketSize = 10000;

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
        _ = Task.Run(StartAsync);
    }

    public void Stop() {
        _ = Task.Run(StopAsync);
    }

    public async Task StartAsync() {

        Logger.LogInfo($"Starting RakNet server on {IpEndPoint}, DEBUG: {Debug}");
        await RunServerLoopAsync();
    }

    public async Task StopAsync() {
        await Server.Stop();
    }

    private async Task RunServerLoopAsync() {

        if (Server.IsRunning) {
            Logger.LogWarn("RakNet is already running.");
            return;
        }

        Logger.LogInfo("RakNet started.");
        var receiveBuffer = new byte[MaxUdpPacketSize];

        Server.Socket.EnableBroadcast = true;
        Server.Socket.Bind(Server.ServerAddress);

        Server.IsRunning = true;
        Server.PacketProcessor = new PacketProcessor(Server.Socket, Server, Server.HandlerSystem);
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (Server.IsRunning) {
            SocketReceiveFromResult received;
            try {
                received = await Server.Socket.ReceiveFromAsync(receiveBuffer, SocketFlags.None, remoteEndPoint);
            } catch (ObjectDisposedException) {
                break;
            } catch (SocketException ex) {
                Logger.LogError($"Socket error while receiving packet: {ex.SocketErrorCode} ({ex.Message})");
                continue;
            }

            if (received.ReceivedBytes == 0)
                continue;

            var packetBuffer = new byte[received.ReceivedBytes];
            Array.Copy(receiveBuffer, packetBuffer, received.ReceivedBytes);

            var packet = DynamicPacketFactory.CreatePacketFromBufferAuto(packetBuffer);
            if (packet is null) {
                Logger.LogWarn($"Failed to parse packet from {received.RemoteEndPoint}");
                continue;
            }

            var clientIpEndPoint = (IPEndPoint)received.RemoteEndPoint;
            var success = await Server.PacketProcessor.ProcessPacketAsync(packet, packetBuffer, clientIpEndPoint);
            if (!success)
                Logger.LogError($"Failed to process packet from {clientIpEndPoint}");
        }
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
