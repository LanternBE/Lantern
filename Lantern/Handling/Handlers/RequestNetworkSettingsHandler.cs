using BedrockProtocol;
using RakSharp.Utils;

namespace Lantern.Handling.Handlers;

public class RequestNetworkSettingsHandler : BedrockPacketHandler<RequestNetworkSettings> {
    
    public override async Task<bool> HandleAsync() {
        
        Logger.LogInfo("Received request network settings!");
        Console.WriteLine(Packet.ProtocolVersion);
        Console.WriteLine(Packet.CompressionAlgorithm);
        return true;
    }
}