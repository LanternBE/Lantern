using BedrockProtocol.Types;
using Lantern.Protocol;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;

namespace Lantern.Utils;

public static class BedrockPacketFactory {
    
    private static readonly List<Func<BedrockPacket>> PacketFactories = [
        () => new BedrockProtocol.RequestNetworkSettings(),
        () => new Login(),
        () => new BedrockProtocol.GamePacket(),
        () => new BedrockProtocol.RawBedrockPacket()
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
            } catch (Exception) {
                //
            }
        }
        
        return null;
    }
}
