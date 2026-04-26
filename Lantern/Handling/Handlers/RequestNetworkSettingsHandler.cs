using BedrockProtocol;
using BedrockProtocol.Types;
using Lantern.Utils;
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
        if (!ProtocolSupport.TryValidateClientProtocol(Packet.ProtocolVersion, out var rejectionStatus, out var logMessage)) {
            Logger.LogInfo(logMessage);
            await SendBedrockPacketAsync(PlayStatus.Create(compressionAlgorithm: Compression.Algorithm.None, rejectionStatus));
            clientSession.Disconnect();
            return false;
        }

        clientSession.Compression = Compression.Algorithm.Zlib;
        // Keep threshold high for now to avoid mandatory compression while basic login flow is incomplete.
        var networkSettings = NetworkSettings.Create(
            compressionAlgorithm: Compression.Algorithm.Zlib,
            compressionThreshold: ushort.MaxValue,
            compressionAlgorithmMethod: 0
        );
        
        await SendBedrockPacketAsync(networkSettings);
        return true;
    }
}
