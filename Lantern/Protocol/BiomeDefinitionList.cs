using BedrockProtocol;
using BedrockProtocol.Types;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;
using BinaryWriter = RakSharp.Binary.BinaryWriter;

namespace Lantern.Protocol;

public class BiomeDefinitionList : BedrockPacket {

    public override Info.BedrockPackets PacketId => Info.BedrockPackets.BiomeDefinitionList;
    public override Compression.Algorithm CompressionAlgorithm { get; set; }

    public uint BiomesCount { get; private set; }
    public uint StringListCount { get; private set; }

    protected override void WriteHeader(BinaryWriter writer) {
        writer.WriteVarUInt((uint)PacketId);
    }

    public override void ReadHeader(BinaryReader reader) {
        var packetId = (int)reader.ReadVarUInt();
        if (packetId != (int)PacketId) {
            throw new RakSharpException.InvalidPacketIdException((uint)PacketId, packetId, nameof(BiomeDefinitionList));
        }
    }

    protected override void WritePayload(BinaryWriter writer) {
        writer.WriteVarUInt(BiomesCount);
        writer.WriteVarUInt(StringListCount);
    }

    protected override void ReadPayload(BinaryReader reader) {
        BiomesCount = reader.ReadVarUInt();
        StringListCount = reader.ReadVarUInt();
    }

    public static (BiomeDefinitionList packet, byte[] buffer) Create(
        Compression.Algorithm compressionAlgorithm
    ) {
        return BedrockPacket.Create<BiomeDefinitionList>(packet => {
            packet.CompressionAlgorithm = compressionAlgorithm;
            packet.BiomesCount = 0;
            packet.StringListCount = 0;
        });
    }
}
