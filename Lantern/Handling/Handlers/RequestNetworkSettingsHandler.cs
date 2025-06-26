using BedrockProtocol;
using BedrockProtocol.Types;
using RakSharp.Utils;
using PlayStatus = BedrockProtocol.PlayStatus;

namespace Lantern.Handling.Handlers;

public class RequestNetworkSettingsHandler : BedrockPacketHandler<RequestNetworkSettings> {
    
    public override async Task<bool> HandleAsync() {
        
        Logger.LogDebug($"Received RequestNetworkSettings inside the GamePacket from ({ClientEndPoint}) with Protocol Version: {Packet.ProtocolVersion}");
        /*if (Packet.ProtocolVersion < Info.SupportedProtocols.Last()) {
            Logger.LogError($"Protocol version {Packet.ProtocolVersion} is outdated");
            return false;
        }

        if (Packet.ProtocolVersion > Info.SupportedProtocols.Last()) {
            Logger.LogError($"Server Protocol version {Packet.ProtocolVersion} is outdated");
            return false;
        }*/

        var playStatus = PlayStatus.Create(Compression.Algorithm.Zlib, BedrockProtocol.Types.PlayStatus.LoginFailedServerOld);
        Console.WriteLine($"Buffer playstatus: {BitConverter.ToString(playStatus.buffer)}");
        await SendBedrockPacketAsync(playStatus);
        
        return true;
    }
}