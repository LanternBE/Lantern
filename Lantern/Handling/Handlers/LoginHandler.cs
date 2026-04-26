using BedrockProtocol;
using BedrockProtocol.Types;
using Lantern.Protocol;
using Lantern.Utils;
using RakSharp.Utils;
using PlayStatus = BedrockProtocol.PlayStatus;

namespace Lantern.Handling.Handlers;

public class LoginHandler : BedrockPacketHandler<Login> {

    public override async Task<bool> HandleAsync() {
        var clientSession = Server.SessionsManager.GetSession(ClientEndPoint);
        if (clientSession is null) {
            Logger.LogDebug($"Ignored Login from ({ClientEndPoint}) without session.");
            return false;
        }

        Logger.LogDebug($"Received Login from ({ClientEndPoint}) with Protocol Version: {Packet.ProtocolVersion}");

        if (!ProtocolSupport.TryValidateClientProtocol(Packet.ProtocolVersion, out var rejectionStatus, out var logMessage)) {
            Logger.LogInfo(logMessage);
            await SendBedrockPacketAsync(PlayStatus.Create(Compression.Algorithm.None, rejectionStatus));
            clientSession.Disconnect();
            return false;
        }

        await SendBedrockPacketAsync(PlayStatus.Create(Compression.Algorithm.Zlib, BedrockProtocol.Types.PlayStatus.LoginSuccess));
        await SendBedrockPacketAsync(ResourcePacksInfo.Create(Compression.Algorithm.Zlib));
        return true;
    }
}
