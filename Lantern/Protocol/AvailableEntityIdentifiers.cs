using BedrockProtocol;
using BedrockProtocol.Types;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;
using BinaryWriter = RakSharp.Binary.BinaryWriter;

namespace Lantern.Protocol;

public class AvailableEntityIdentifiers : BedrockPacket {

    public override Info.BedrockPackets PacketId => Info.BedrockPackets.AvailableEntityIdentifiers;
    public override Compression.Algorithm CompressionAlgorithm { get; set; }

    // Network little-endian NBT compound payload.
    public byte[] SerializedEntityIdentifiers { get; private set; } = [0x0A, 0x00, 0x00, 0x00];

    protected override void WriteHeader(BinaryWriter writer) {
        writer.WriteVarUInt((uint)PacketId);
    }

    public override void ReadHeader(BinaryReader reader) {
        var packetId = (int)reader.ReadVarUInt();
        if (packetId != (int)PacketId) {
            throw new RakSharpException.InvalidPacketIdException((uint)PacketId, packetId, nameof(AvailableEntityIdentifiers));
        }
    }

    protected override void WritePayload(BinaryWriter writer) {
        writer.Write(SerializedEntityIdentifiers);
    }

    protected override void ReadPayload(BinaryReader reader) {
        SerializedEntityIdentifiers = reader.ReadRemainingBytes();
    }

    public static (AvailableEntityIdentifiers packet, byte[] buffer) Create(
        Compression.Algorithm compressionAlgorithm,
        byte[]? serializedEntityIdentifiers = null
    ) {
        return BedrockPacket.Create<AvailableEntityIdentifiers>(packet => {
            packet.CompressionAlgorithm = compressionAlgorithm;
            packet.SerializedEntityIdentifiers = serializedEntityIdentifiers ?? [0x0A, 0x00, 0x00, 0x00];
        });
    }
}
