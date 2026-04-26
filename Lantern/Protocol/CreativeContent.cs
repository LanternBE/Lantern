using BedrockProtocol;
using BedrockProtocol.Types;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;
using BinaryWriter = RakSharp.Binary.BinaryWriter;

namespace Lantern.Protocol;

public class CreativeContent : BedrockPacket {

    public override Info.BedrockPackets PacketId => Info.BedrockPackets.CreativeContent;
    public override Compression.Algorithm CompressionAlgorithm { get; set; }

    public uint GroupsCount { get; private set; }
    public uint ItemsCount { get; private set; }

    protected override void WriteHeader(BinaryWriter writer) {
        writer.WriteVarUInt((uint)PacketId);
    }

    public override void ReadHeader(BinaryReader reader) {
        var packetId = (int)reader.ReadVarUInt();
        if (packetId != (int)PacketId) {
            throw new RakSharpException.InvalidPacketIdException((uint)PacketId, packetId, nameof(CreativeContent));
        }
    }

    protected override void WritePayload(BinaryWriter writer) {
        writer.WriteVarUInt(GroupsCount);
        writer.WriteVarUInt(ItemsCount);
    }

    protected override void ReadPayload(BinaryReader reader) {
        GroupsCount = reader.ReadVarUInt();
        ItemsCount = reader.ReadVarUInt();
    }

    public static (CreativeContent packet, byte[] buffer) Create(
        Compression.Algorithm compressionAlgorithm
    ) {
        return BedrockPacket.Create<CreativeContent>(packet => {
            packet.CompressionAlgorithm = compressionAlgorithm;
            packet.GroupsCount = 0;
            packet.ItemsCount = 0;
        });
    }
}
