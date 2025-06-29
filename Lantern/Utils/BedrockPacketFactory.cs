using BedrockProtocol.Types;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;

namespace Lantern.Utils;

public static class BedrockPacketFactory {
    
    private static readonly List<Type> PacketTypes = [
        typeof(BedrockProtocol.RequestNetworkSettings),
        typeof(BedrockProtocol.GamePacket)
    ];

    public static BedrockPacket? CreateFromBuffer(byte[] buffer) {
        
        if (buffer is null || buffer.Length is 0)
            return null;

        foreach (var type in PacketTypes) {
            
            try {
                var reader = new BinaryReader(buffer);
                var packet = (BedrockPacket)Activator.CreateInstance(type)!;
                reader.Position = 0;
                
                packet.Read(reader);
                return packet;
            } catch (RakSharpException.InvalidPacketIdException e) {
                //
            }
        }
        
        return null;
    }
}