using BedrockProtocol;
using BedrockProtocol.Types;
using Lantern.Protocol;
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

        if (Info.SupportedProtocols.Count == 0) {
            Logger.LogError("No supported protocol configured on server");
            clientSession.Disconnect();
            return false;
        }

        var minSupportedProtocol = Info.SupportedProtocols.Min();
        var maxSupportedProtocol = Info.SupportedProtocols.Max();

        if (Packet.ProtocolVersion < minSupportedProtocol) {
            Logger.LogInfo($"Client protocol version {Packet.ProtocolVersion} is outdated");
            await SendBedrockPacketAsync(PlayStatus.Create(Compression.Algorithm.None, BedrockProtocol.Types.PlayStatus.LoginFailedClientOld));
            clientSession.Disconnect();
            return false;
        }

        if (Packet.ProtocolVersion > maxSupportedProtocol) {
            Logger.LogInfo($"Client protocol version {Packet.ProtocolVersion} is newer than supported by server");
            await SendBedrockPacketAsync(PlayStatus.Create(Compression.Algorithm.None, BedrockProtocol.Types.PlayStatus.LoginFailedServerOld));
            clientSession.Disconnect();
            return false;
        }

        await SendBedrockPacketAsync(PlayStatus.Create(Compression.Algorithm.Zlib, BedrockProtocol.Types.PlayStatus.LoginSuccess));
        return true;
    }
}
