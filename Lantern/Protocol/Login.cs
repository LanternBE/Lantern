using BedrockProtocol;
using BedrockProtocol.Types;
using RakSharp;
using BinaryReader = RakSharp.Binary.BinaryReader;
using BinaryWriter = RakSharp.Binary.BinaryWriter;

namespace Lantern.Protocol;

public class Login : BedrockPacket {

    public override Info.BedrockPackets PacketId => Info.BedrockPackets.Login;
    public override Compression.Algorithm CompressionAlgorithm { get; set; }

    public int ProtocolVersion { get; private set; }
    public byte[] Payload { get; private set; } = [];

    protected override void WriteHeader(BinaryWriter writer) {
        writer.WriteVarUInt((uint)PacketId);
    }

    public override void ReadHeader(BinaryReader reader) {
        var packetId = (int)reader.ReadVarUInt();
        if (packetId != (int)PacketId) {
            throw new RakSharpException.InvalidPacketIdException((uint)PacketId, packetId, nameof(Login));
        }
    }

    protected override void WritePayload(BinaryWriter writer) {
        writer.WriteIntBigEndian(ProtocolVersion);
        writer.Write(Payload);
    }

    protected override void ReadPayload(BinaryReader reader) {
        ProtocolVersion = reader.ReadIntBigEndian();
        Payload = reader.ReadRemainingBytes();
    }
}
