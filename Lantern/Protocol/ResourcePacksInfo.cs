using System.Text;
using BedrockProtocol;
using BedrockProtocol.Types;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;
using BinaryWriter = RakSharp.Binary.BinaryWriter;

namespace Lantern.Protocol;

public class ResourcePacksInfo : BedrockPacket {

    public override Info.BedrockPackets PacketId => Info.BedrockPackets.ResourcePacksInfo;
    public override Compression.Algorithm CompressionAlgorithm { get; set; }

    public bool MustAccept { get; private set; }
    public bool HasAddons { get; private set; }
    public bool HasScripts { get; private set; }
    public bool ForceDisableVibrantVisuals { get; private set; }
    public byte[] WorldTemplateId { get; private set; } = new byte[16];
    public string WorldTemplateVersion { get; private set; } = string.Empty;
    public ushort ResourcePackCount { get; private set; }

    protected override void WriteHeader(BinaryWriter writer) {
        writer.WriteVarUInt((uint)PacketId);
    }

    public override void ReadHeader(BinaryReader reader) {
        var packetId = (int)reader.ReadVarUInt();
        if (packetId != (int)PacketId) {
            throw new RakSharpException.InvalidPacketIdException((uint)PacketId, packetId, nameof(ResourcePacksInfo));
        }
    }

    protected override void WritePayload(BinaryWriter writer) {
        writer.WriteBoolean(MustAccept);
        writer.WriteBoolean(HasAddons);
        writer.WriteBoolean(HasScripts);
        writer.WriteBoolean(ForceDisableVibrantVisuals);
        writer.Write(WorldTemplateId);
        WriteBedrockString(writer, WorldTemplateVersion);
        writer.WriteUnsignedShortLittleEndian(ResourcePackCount);
    }

    protected override void ReadPayload(BinaryReader reader) {
        MustAccept = reader.ReadBoolean();
        HasAddons = reader.ReadBoolean();
        HasScripts = reader.ReadBoolean();
        ForceDisableVibrantVisuals = reader.ReadBoolean();
        WorldTemplateId = reader.ReadBytes(16);
        WorldTemplateVersion = ReadBedrockString(reader);
        ResourcePackCount = reader.ReadUnsignedShortLittleEndian();
    }

    public static (ResourcePacksInfo packet, byte[] buffer) Create(
        Compression.Algorithm compressionAlgorithm,
        bool mustAccept = false,
        bool hasAddons = false,
        bool hasScripts = false,
        bool forceDisableVibrantVisuals = false
    ) {
        return BedrockPacket.Create<ResourcePacksInfo>(packet => {
            packet.CompressionAlgorithm = compressionAlgorithm;
            packet.MustAccept = mustAccept;
            packet.HasAddons = hasAddons;
            packet.HasScripts = hasScripts;
            packet.ForceDisableVibrantVisuals = forceDisableVibrantVisuals;
            packet.WorldTemplateId = new byte[16];
            packet.WorldTemplateVersion = string.Empty;
            packet.ResourcePackCount = 0;
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
