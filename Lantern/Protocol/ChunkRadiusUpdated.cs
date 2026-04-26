using BedrockProtocol;
using BedrockProtocol.Types;
using Lantern.Utils;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;
using BinaryWriter = RakSharp.Binary.BinaryWriter;

namespace Lantern.Protocol;

public class ChunkRadiusUpdated : BedrockPacket {

    public override Info.BedrockPackets PacketId => Info.BedrockPackets.ChunkRadiusUpdate;
    public override Compression.Algorithm CompressionAlgorithm { get; set; }

    public int ChunkRadius { get; private set; }

    protected override void WriteHeader(BinaryWriter writer) {
        writer.WriteVarUInt((uint)PacketId);
    }

    public override void ReadHeader(BinaryReader reader) {
        var packetId = (int)reader.ReadVarUInt();
        if (packetId != (int)PacketId) {
            throw new RakSharpException.InvalidPacketIdException((uint)PacketId, packetId, nameof(ChunkRadiusUpdated));
        }
    }

    protected override void WritePayload(BinaryWriter writer) {
        BedrockVarInt.WriteVarInt32(writer, ChunkRadius);
    }

    protected override void ReadPayload(BinaryReader reader) {
        ChunkRadius = BedrockVarInt.ReadVarInt32(reader);
    }

    public static (ChunkRadiusUpdated packet, byte[] buffer) Create(
        Compression.Algorithm compressionAlgorithm,
        int chunkRadius
    ) {
        return BedrockPacket.Create<ChunkRadiusUpdated>(packet => {
            packet.CompressionAlgorithm = compressionAlgorithm;
            packet.ChunkRadius = chunkRadius;
        });
    }
}
