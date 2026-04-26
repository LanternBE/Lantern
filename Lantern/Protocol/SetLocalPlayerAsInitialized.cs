using BedrockProtocol;
using BedrockProtocol.Types;
using Lantern.Utils;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;
using BinaryWriter = RakSharp.Binary.BinaryWriter;

namespace Lantern.Protocol;

public class SetLocalPlayerAsInitialized : BedrockPacket {

    public override Info.BedrockPackets PacketId => Info.BedrockPackets.SetLocalPlayerAsInitialized;
    public override Compression.Algorithm CompressionAlgorithm { get; set; }

    public ulong EntityRuntimeId { get; private set; }

    protected override void WriteHeader(BinaryWriter writer) {
        writer.WriteVarUInt((uint)PacketId);
    }

    public override void ReadHeader(BinaryReader reader) {
        var packetId = (int)reader.ReadVarUInt();
        if (packetId != (int)PacketId) {
            throw new RakSharpException.InvalidPacketIdException((uint)PacketId, packetId, nameof(SetLocalPlayerAsInitialized));
        }
    }

    protected override void WritePayload(BinaryWriter writer) {
        writer.WriteVarUInt(EntityRuntimeId);
    }

    protected override void ReadPayload(BinaryReader reader) {
        EntityRuntimeId = BedrockVarInt.ReadVarUInt64(reader);
    }
}
