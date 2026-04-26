using BedrockProtocol.Types;
using Lantern.Protocol;
using RakSharp;
using RakSharp.Utils;
using BinaryReader = RakSharp.Binary.BinaryReader;

namespace Lantern.Utils;

public static class BedrockPacketFactory {
    
    private static readonly List<Func<BedrockPacket>> PacketFactories = [
        () => new BedrockProtocol.RequestNetworkSettings(),
        () => new Login(),
        () => new ResourcePackClientResponse(),
        () => new BedrockProtocol.GamePacket(),
        () => new BedrockProtocol.RawBedrockPacket()
    ];

    public static BedrockPacket? CreateFromBuffer(byte[] buffer) {
        
        if (buffer is null || buffer.Length is 0)
            return null;

        foreach (var packetFactory in PacketFactories) {
            BedrockPacket? packet = null;
            try {
                var reader = new BinaryReader(buffer);
                packet = packetFactory();
                reader.Position = 0;
                
                packet.Read(reader);
                return packet;
            } catch (RakSharpException.InvalidPacketIdException) {
                //
            } catch (RakSharpException ex) {
                Logger.LogDebug($"Failed to parse packet with {(packet?.GetType().Name ?? "UnknownPacket")} ({ex.GetType().Name}): {ex.Message}");
            } catch (Exception ex) {
                Logger.LogWarn($"Unexpected error while parsing {(packet?.GetType().Name ?? "UnknownPacket")}: {ex.GetType().Name}: {ex.Message}");
            }
        }
        
        return null;
    }
}
