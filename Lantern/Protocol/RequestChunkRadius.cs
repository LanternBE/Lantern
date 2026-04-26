using BedrockProtocol;
using BedrockProtocol.Types;
using Lantern.Utils;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;
using BinaryWriter = RakSharp.Binary.BinaryWriter;

namespace Lantern.Protocol;

public class RequestChunkRadius : BedrockPacket {

    public override Info.BedrockPackets PacketId => Info.BedrockPackets.RequestChunkRadius;
    public override Compression.Algorithm CompressionAlgorithm { get; set; }

    public int ChunkRadius { get; private set; }
    public byte MaxChunkRadius { get; private set; }

    protected override void WriteHeader(BinaryWriter writer) {
        writer.WriteVarUInt((uint)PacketId);
    }

    public override void ReadHeader(BinaryReader reader) {
        var packetId = (int)reader.ReadVarUInt();
        if (packetId != (int)PacketId) {
            throw new RakSharpException.InvalidPacketIdException((uint)PacketId, packetId, nameof(RequestChunkRadius));
        }
    }

    protected override void WritePayload(BinaryWriter writer) {
        BedrockVarInt.WriteVarInt32(writer, ChunkRadius);
        writer.WriteByte(MaxChunkRadius);
    }

    protected override void ReadPayload(BinaryReader reader) {
        ChunkRadius = BedrockVarInt.ReadVarInt32(reader);
        // Some clients/protocol revisions omit MaxChunkRadius and only send ChunkRadius.
        // Defaulting to 0 keeps compatibility while preserving the requested radius.
        MaxChunkRadius = reader.IsAtEnd() ? (byte)0 : reader.ReadByte();
    }
}
