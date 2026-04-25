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

        if (Packet.ProtocolVersion < Info.SupportedProtocols.First()) {
            Logger.LogError($"Protocol version {Packet.ProtocolVersion} is outdated");
            await SendBedrockPacketAsync(PlayStatus.Create(Compression.Algorithm.None, BedrockProtocol.Types.PlayStatus.LoginFailedClientOld));
            clientSession.Disconnect();
            return false;
        }

        if (Packet.ProtocolVersion > Info.SupportedProtocols.Last()) {
            Logger.LogError($"Server Protocol version {Packet.ProtocolVersion} is outdated");
            await SendBedrockPacketAsync(PlayStatus.Create(Compression.Algorithm.None, BedrockProtocol.Types.PlayStatus.LoginFailedServerOld));
            clientSession.Disconnect();
            return false;
        }

        await SendBedrockPacketAsync(PlayStatus.Create(Compression.Algorithm.Zlib, BedrockProtocol.Types.PlayStatus.LoginSuccess));
        return true;
    }
}
