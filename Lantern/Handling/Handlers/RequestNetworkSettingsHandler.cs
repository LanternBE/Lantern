using BedrockProtocol;
using BedrockProtocol.Types;
using RakSharp.Utils;

namespace Lantern.Handling.Handlers;

public class RequestNetworkSettingsHandler : BedrockPacketHandler<RequestNetworkSettings> {
    
    public override async Task<bool> HandleAsync() {
        
        var clientSession = Server.SessionsManager.GetSession(ClientEndPoint);
        if (clientSession is null) {
            Logger.LogDebug($"Ignored a RequestNetworkSettings from a client ({ClientEndPoint}) without session.");
            return false;
        }
        
        Logger.LogDebug($"Received RequestNetworkSettings inside the GamePacket from ({ClientEndPoint}) with Protocol Version: {Packet.ProtocolVersion}");
        if (Packet.ProtocolVersion < Info.SupportedProtocols.Last()) {
            Logger.LogError($"Protocol version {Packet.ProtocolVersion} is outdated");
            // TODO: Send PlayStatus packet with BedrockProtocol.Types.PlayStatus.LoginFailedClientOld
            return false;
        }

        if (Packet.ProtocolVersion > Info.SupportedProtocols.Last()) {
            Logger.LogError($"Server Protocol version {Packet.ProtocolVersion} is outdated");
            // TODO: Send PlayStatus packet with BedrockProtocol.Types.PlayStatus.LoginFailedServerOld
            return false;
        }

        clientSession.Compression = Compression.Algorithm.Zlib;
        var networkSettings = NetworkSettings.Create(Compression.Algorithm.Zlib);
        
        await SendBedrockPacketAsync(networkSettings);
        Console.WriteLine(BitConverter.ToString(networkSettings.buffer));
        return true;
    }
}