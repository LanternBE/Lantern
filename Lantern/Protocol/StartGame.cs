using BedrockProtocol;
using BedrockProtocol.Types;
using Lantern.Utils;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;
using BinaryWriter = RakSharp.Binary.BinaryWriter;

namespace Lantern.Protocol;

public class StartGame : BedrockPacket {

    private static readonly byte[] EmptyNetworkLittleEndianCompound = [0x0A, 0x00, 0x00, 0x00];

    public override Info.BedrockPackets PacketId => Info.BedrockPackets.StartGame;
    public override Compression.Algorithm CompressionAlgorithm { get; set; }

    public long EntityUniqueId { get; private set; }
    public ulong EntityRuntimeId { get; private set; }

    public int PlayerGameMode { get; private set; }
    public float PlayerX { get; private set; }
    public float PlayerY { get; private set; }
    public float PlayerZ { get; private set; }
    public float Pitch { get; private set; }
    public float Yaw { get; private set; }

    public string LevelId { get; private set; } = string.Empty;
    public string WorldName { get; private set; } = "Lantern";
    public string MultiPlayerCorrelationId { get; private set; } = string.Empty;
    public string GameVersion { get; private set; } = ProtocolSupport.BaseGameVersion;

    protected override void WriteHeader(BinaryWriter writer) {
        writer.WriteVarUInt((uint)PacketId);
    }

    public override void ReadHeader(BinaryReader reader) {
        var packetId = (int)reader.ReadVarUInt();
        if (packetId != (int)PacketId) {
            throw new RakSharpException.InvalidPacketIdException((uint)PacketId, packetId, nameof(StartGame));
        }
    }

    protected override void WritePayload(BinaryWriter writer) {
        BedrockVarInt.WriteVarInt64(writer, EntityUniqueId);
        writer.WriteVarUInt(EntityRuntimeId);
        BedrockVarInt.WriteVarInt32(writer, PlayerGameMode);

        writer.WriteFloatLittleEndian(PlayerX);
        writer.WriteFloatLittleEndian(PlayerY);
        writer.WriteFloatLittleEndian(PlayerZ);
        writer.WriteFloatLittleEndian(Pitch);
        writer.WriteFloatLittleEndian(Yaw);

        writer.WriteLongLittleEndian(0); // WorldSeed
        writer.WriteShortLittleEndian(0); // SpawnBiomeType default
        BedrockStringCodec.Write(writer, string.Empty); // UserDefinedBiomeName

        BedrockVarInt.WriteVarInt32(writer, 0); // Dimension (Overworld)
        BedrockVarInt.WriteVarInt32(writer, 1); // Generator (Infinite)
        BedrockVarInt.WriteVarInt32(writer, 0); // WorldGameMode (Survival)
        writer.WriteBoolean(false); // Hardcore
        BedrockVarInt.WriteVarInt32(writer, 2); // Difficulty (Normal)

        BedrockVarInt.WriteVarInt32(writer, 0); // WorldSpawn X
        BedrockVarInt.WriteVarInt32(writer, (int)PlayerY); // WorldSpawn Y
        BedrockVarInt.WriteVarInt32(writer, 0); // WorldSpawn Z

        writer.WriteBoolean(false); // AchievementsDisabled
        BedrockVarInt.WriteVarInt32(writer, 0); // EditorWorldType
        writer.WriteBoolean(false); // CreatedInEditor
        writer.WriteBoolean(false); // ExportedFromEditor
        BedrockVarInt.WriteVarInt32(writer, 0); // DayCycleLockTime
        BedrockVarInt.WriteVarInt32(writer, 0); // EducationEditionOffer
        writer.WriteBoolean(false); // EducationFeaturesEnabled
        BedrockStringCodec.Write(writer, string.Empty); // EducationProductID
        writer.WriteFloatLittleEndian(0); // RainLevel
        writer.WriteFloatLittleEndian(0); // LightningLevel
        writer.WriteBoolean(false); // ConfirmedPlatformLockedContent

        writer.WriteBoolean(true); // MultiPlayerGame
        writer.WriteBoolean(true); // LANBroadcastEnabled
        BedrockVarInt.WriteVarInt32(writer, 4); // XBLBroadcastMode public
        BedrockVarInt.WriteVarInt32(writer, 4); // PlatformBroadcastMode public
        writer.WriteBoolean(true); // CommandsEnabled
        writer.WriteBoolean(false); // TexturePackRequired

        writer.WriteVarUInt(0); // GameRules (legacy) count
        writer.WriteUnsignedIntLittleEndian(0); // Experiments count (uint32)
        writer.WriteBoolean(false); // ExperimentsPreviouslyToggled

        writer.WriteBoolean(false); // BonusChestEnabled
        writer.WriteBoolean(false); // StartWithMapEnabled
        BedrockVarInt.WriteVarInt32(writer, 1); // PlayerPermissions (Member)
        writer.WriteIntLittleEndian(4); // ServerChunkTickRadius

        writer.WriteBoolean(false); // HasLockedBehaviourPack
        writer.WriteBoolean(false); // HasLockedTexturePack
        writer.WriteBoolean(false); // FromLockedWorldTemplate
        writer.WriteBoolean(false); // MSAGamerTagsOnly
        writer.WriteBoolean(false); // FromWorldTemplate
        writer.WriteBoolean(true); // WorldTemplateSettingsLocked
        writer.WriteBoolean(false); // OnlySpawnV1Villagers
        writer.WriteBoolean(false); // PersonaDisabled
        writer.WriteBoolean(false); // CustomSkinsDisabled
        writer.WriteBoolean(false); // EmoteChatMuted

        BedrockStringCodec.Write(writer, ProtocolSupport.BaseGameVersion); // BaseGameVersion
        writer.WriteIntLittleEndian(0); // LimitedWorldWidth
        writer.WriteIntLittleEndian(0); // LimitedWorldDepth
        writer.WriteBoolean(true); // NewNether

        BedrockStringCodec.Write(writer, string.Empty); // EducationSharedResourceURI.ButtonName
        BedrockStringCodec.Write(writer, string.Empty); // EducationSharedResourceURI.LinkURI

        writer.WriteBoolean(false); // ForceExperimentalGameplay optional present
        writer.WriteByte(0); // ChatRestrictionLevel (none)
        writer.WriteBoolean(false); // DisablePlayerInteractions

        BedrockStringCodec.Write(writer, LevelId);
        BedrockStringCodec.Write(writer, WorldName);
        BedrockStringCodec.Write(writer, string.Empty); // TemplateContentIdentity
        writer.WriteBoolean(false); // Trial

        BedrockVarInt.WriteVarInt32(writer, 0); // PlayerMovementSettings.RewindHistorySize
        writer.WriteBoolean(false); // PlayerMovementSettings.ServerAuthoritativeBlockBreaking

        writer.WriteLongLittleEndian(0); // Time
        BedrockVarInt.WriteVarInt32(writer, 0); // EnchantmentSeed
        writer.WriteVarUInt(0); // Blocks count

        BedrockStringCodec.Write(writer, MultiPlayerCorrelationId);
        writer.WriteBoolean(true); // ServerAuthoritativeInventory
        BedrockStringCodec.Write(writer, GameVersion);

        writer.Write(EmptyNetworkLittleEndianCompound); // PropertyData NBT
        writer.WriteUnsignedLongLittleEndian(0); // ServerBlockStateChecksum

        writer.Write(new byte[16]); // WorldTemplateID UUID
        writer.WriteBoolean(false); // ClientSideGeneration
        writer.WriteBoolean(false); // UseBlockNetworkIDHashes
        writer.WriteBoolean(false); // ServerAuthoritativeSound

        writer.WriteBoolean(false); // Optional ServerJoinInformation not present

        BedrockStringCodec.Write(writer, string.Empty); // ServerID
        BedrockStringCodec.Write(writer, string.Empty); // ScenarioID
        BedrockStringCodec.Write(writer, string.Empty); // WorldID
        BedrockStringCodec.Write(writer, string.Empty); // OwnerID
    }

    protected override void ReadPayload(BinaryReader reader) {
        // Server-only packet in current implementation.
        reader.ReadRemainingBytes();
    }

    public static (StartGame packet, byte[] buffer) Create(
        Compression.Algorithm compressionAlgorithm,
        ulong entityRuntimeId,
        string worldName,
        int playerGameMode = 0,
        float x = 0,
        float y = 64,
        float z = 0
    ) {
        return BedrockPacket.Create<StartGame>(packet => {
            packet.CompressionAlgorithm = compressionAlgorithm;
            packet.EntityUniqueId = (long)entityRuntimeId;
            packet.EntityRuntimeId = entityRuntimeId;
            packet.PlayerGameMode = playerGameMode;
            packet.PlayerX = x;
            packet.PlayerY = y;
            packet.PlayerZ = z;
            packet.Pitch = 0;
            packet.Yaw = 0;
            packet.LevelId = Guid.NewGuid().ToString("N");
            packet.WorldName = worldName;
            packet.MultiPlayerCorrelationId = Guid.NewGuid().ToString();
            packet.GameVersion = ProtocolSupport.BaseGameVersion;
        });
    }
}
