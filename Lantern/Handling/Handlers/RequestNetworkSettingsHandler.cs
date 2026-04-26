using BedrockProtocol;
using BedrockProtocol.Types;
using RakSharp.Utils;
using PlayStatus = BedrockProtocol.PlayStatus;

namespace Lantern.Handling.Handlers;

public class RequestNetworkSettingsHandler : BedrockPacketHandler<RequestNetworkSettings> {
    
    public override async Task<bool> HandleAsync() {
        
        var clientSession = Server.SessionsManager.GetSession(ClientEndPoint);
        if (clientSession is null) {
            Logger.LogDebug($"Ignored a RequestNetworkSettings from a client ({ClientEndPoint}) without session.");
            return false;
        }
        
        Logger.LogDebug($"Received RequestNetworkSettings inside the GamePacket from ({ClientEndPoint}) with Protocol Version: {Packet.ProtocolVersion}");
        if (Info.SupportedProtocols.Count == 0) {
            Logger.LogError("No supported protocol configured on server");
            clientSession.Disconnect();
            return false;
        }

        var minSupportedProtocol = Info.SupportedProtocols.Min();
        var maxSupportedProtocol = Info.SupportedProtocols.Max();

        if (Packet.ProtocolVersion < minSupportedProtocol) {
            
            Logger.LogInfo($"Client protocol version {Packet.ProtocolVersion} is outdated");
            await SendBedrockPacketAsync(PlayStatus.Create(compressionAlgorithm: Compression.Algorithm.None, BedrockProtocol.Types.PlayStatus.LoginFailedClientOld));
            
            clientSession.Disconnect();
            return false;
        }

        if (Packet.ProtocolVersion > maxSupportedProtocol) {
            
            Logger.LogInfo($"Client protocol version {Packet.ProtocolVersion} is newer than supported by server");
            await SendBedrockPacketAsync(PlayStatus.Create(compressionAlgorithm: Compression.Algorithm.None, BedrockProtocol.Types.PlayStatus.LoginFailedServerOld));
           
            clientSession.Disconnect();
            return false;
        }

        clientSession.Compression = Compression.Algorithm.Zlib;
        var networkSettings = NetworkSettings.Create(
            compressionAlgorithm: Compression.Algorithm.Zlib,
            compressionThreshold: ushort.MaxValue,
            compressionAlgorithmMethod: 0
        );
        
        await SendBedrockPacketAsync(networkSettings);
        return true;
    }
}
