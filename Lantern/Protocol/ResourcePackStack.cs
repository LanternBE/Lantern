using BedrockProtocol;
using BedrockProtocol.Types;
using Lantern.Utils;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;
using BinaryWriter = RakSharp.Binary.BinaryWriter;

namespace Lantern.Protocol;

public class ResourcePackStack : BedrockPacket {

    public override Info.BedrockPackets PacketId => Info.BedrockPackets.ResourcePackStack;
    public override Compression.Algorithm CompressionAlgorithm { get; set; }

    public bool MustAccept { get; private set; }
    public uint ResourcePackStackCount { get; private set; }
    public string BaseGameVersion { get; private set; } = "1.21.90";
    public uint ExperimentsCount { get; private set; }
    public bool ExperimentsPreviouslyToggled { get; private set; }
    public bool UseVanillaEditorPacks { get; private set; }

    protected override void WriteHeader(BinaryWriter writer) {
        writer.WriteVarUInt((uint)PacketId);
    }

    public override void ReadHeader(BinaryReader reader) {
        var packetId = (int)reader.ReadVarUInt();
        if (packetId != (int)PacketId) {
            throw new RakSharpException.InvalidPacketIdException((uint)PacketId, packetId, nameof(ResourcePackStack));
        }
    }

    protected override void WritePayload(BinaryWriter writer) {
        writer.WriteBoolean(MustAccept);
        writer.WriteVarUInt(ResourcePackStackCount);
        BedrockStringCodec.Write(writer, BaseGameVersion);
        writer.WriteVarUInt(ExperimentsCount);
        writer.WriteBoolean(ExperimentsPreviouslyToggled);
        writer.WriteBoolean(UseVanillaEditorPacks);
    }

    protected override void ReadPayload(BinaryReader reader) {
        MustAccept = reader.ReadBoolean();
        ResourcePackStackCount = reader.ReadVarUInt();
        BaseGameVersion = BedrockStringCodec.Read(reader);
        ExperimentsCount = reader.ReadVarUInt();
        ExperimentsPreviouslyToggled = reader.ReadBoolean();
        UseVanillaEditorPacks = reader.ReadBoolean();
    }

    public static (ResourcePackStack packet, byte[] buffer) Create(
        Compression.Algorithm compressionAlgorithm,
        bool mustAccept = false,
        string baseGameVersion = "1.21.90",
        bool useVanillaEditorPacks = false
    ) {
        return BedrockPacket.Create<ResourcePackStack>(packet => {
            packet.CompressionAlgorithm = compressionAlgorithm;
            packet.MustAccept = mustAccept;
            packet.ResourcePackStackCount = 0;
            packet.BaseGameVersion = baseGameVersion;
            packet.ExperimentsCount = 0;
            packet.ExperimentsPreviouslyToggled = false;
            packet.UseVanillaEditorPacks = useVanillaEditorPacks;
        });
    }

}
