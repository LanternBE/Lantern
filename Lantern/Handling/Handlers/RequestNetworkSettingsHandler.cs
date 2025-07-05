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
        if (Packet.ProtocolVersion < Info.SupportedProtocols.First()) {
            
            Logger.LogError($"Protocol version {Packet.ProtocolVersion} is outdated");
            await SendBedrockPacketAsync(PlayStatus.Create(compressionAlgorithm: Compression.Algorithm.None, BedrockProtocol.Types.PlayStatus.LoginFailedClientOld));
            
            clientSession.Disconnect();
            return false;
        }

        if (Packet.ProtocolVersion > Info.SupportedProtocols.Last()) {
            
            Logger.LogError($"Server Protocol version {Packet.ProtocolVersion} is outdated");
            await SendBedrockPacketAsync(PlayStatus.Create(compressionAlgorithm: Compression.Algorithm.None, BedrockProtocol.Types.PlayStatus.LoginFailedServerOld));
           
            clientSession.Disconnect();
            return false;
        }

        clientSession.Compression = Compression.Algorithm.Zlib;
        var networkSettings = NetworkSettings.Create(Compression.Algorithm.Zlib);
        
        await SendBedrockPacketAsync(networkSettings);
        Console.WriteLine(BitConverter.ToString(networkSettings.buffer));
        
        return true;
    }
}