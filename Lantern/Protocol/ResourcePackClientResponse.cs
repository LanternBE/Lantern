using System.Text;
using BedrockProtocol;
using BedrockProtocol.Types;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;
using BinaryWriter = RakSharp.Binary.BinaryWriter;

namespace Lantern.Protocol;

public class ResourcePackClientResponse : BedrockPacket {

    public const byte StatusRefused = 1;
    public const byte StatusSendPacks = 2;
    public const byte StatusHaveAllPacks = 3;
    public const byte StatusCompleted = 4;

    public override Info.BedrockPackets PacketId => Info.BedrockPackets.ResourcePackClientResponse;
    public override Compression.Algorithm CompressionAlgorithm { get; set; }

    public byte Status { get; private set; }
    public List<string> PackIds { get; private set; } = [];

    protected override void WriteHeader(BinaryWriter writer) {
        writer.WriteVarUInt((uint)PacketId);
    }

    public override void ReadHeader(BinaryReader reader) {
        var packetId = (int)reader.ReadVarUInt();
        if (packetId != (int)PacketId) {
            throw new RakSharpException.InvalidPacketIdException((uint)PacketId, packetId, nameof(ResourcePackClientResponse));
        }
    }

    protected override void WritePayload(BinaryWriter writer) {
        writer.WriteByte(Status);
        writer.WriteUnsignedShortLittleEndian((ushort)PackIds.Count);
        foreach (var packId in PackIds) {
            WriteBedrockString(writer, packId);
        }
    }

    protected override void ReadPayload(BinaryReader reader) {
        Status = reader.ReadByte();
        var count = reader.ReadUnsignedShortLittleEndian();

        PackIds = [];
        for (var i = 0; i < count; i++) {
            PackIds.Add(ReadBedrockString(reader));
        }
    }

    public static (ResourcePackClientResponse packet, byte[] buffer) Create(
        Compression.Algorithm compressionAlgorithm,
        byte status,
        IEnumerable<string>? packIds = null
    ) {
        return BedrockPacket.Create<ResourcePackClientResponse>(packet => {
            packet.CompressionAlgorithm = compressionAlgorithm;
            packet.Status = status;
            packet.PackIds = packIds?.ToList() ?? [];
        });
    }

    private static string ReadBedrockString(BinaryReader reader) {
        var length = (int)reader.ReadVarUInt();
        if (length <= 0) {
            return string.Empty;
        }

        var bytes = reader.ReadBytes(length);
        return Encoding.UTF8.GetString(bytes);
    }

    private static void WriteBedrockString(BinaryWriter writer, string value) {
        var bytes = Encoding.UTF8.GetBytes(value);
        writer.WriteVarUInt((uint)bytes.Length);
        writer.Write(bytes);
    }
}
