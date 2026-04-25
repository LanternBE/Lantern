using BedrockProtocol.Types;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;

namespace Lantern.Utils;

public static class BedrockPacketFactory {
    
    private static readonly List<Func<BedrockPacket>> PacketFactories = [
        () => new BedrockProtocol.RequestNetworkSettings(),
        () => new BedrockProtocol.GamePacket()
    ];

    public static BedrockPacket? CreateFromBuffer(byte[] buffer) {
        
        if (buffer is null || buffer.Length is 0)
            return null;

        foreach (var packetFactory in PacketFactories) {
            
            try {
                var reader = new BinaryReader(buffer);
                var packet = packetFactory();
                reader.Position = 0;
                
                packet.Read(reader);
                return packet;
            } catch (RakSharpException.InvalidPacketIdException) {
                //
            }
        }
        
        return null;
    }
}
