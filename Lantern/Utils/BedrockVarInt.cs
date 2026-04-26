using BinaryReader = RakSharp.Binary.BinaryReader;
using BinaryWriter = RakSharp.Binary.BinaryWriter;

namespace Lantern.Utils;

public static class BedrockVarInt {

    public static void WriteVarInt32(BinaryWriter writer, int value) {
        var zigZag = (uint)((value << 1) ^ (value >> 31));
        writer.WriteVarUInt(zigZag);
    }

    public static int ReadVarInt32(BinaryReader reader) {
        var value = reader.ReadVarUInt();
        return (int)((value >> 1) ^ (uint)-(int)(value & 1));
    }

    public static void WriteVarInt64(BinaryWriter writer, long value) {
        var zigZag = (ulong)((value << 1) ^ (value >> 63));
        writer.WriteVarUInt(zigZag);
    }

    public static long ReadVarInt64(BinaryReader reader) {
        var value = ReadVarUInt64(reader);
        return (long)((value >> 1) ^ (ulong)-(long)(value & 1));
    }

    public static ulong ReadVarUInt64(BinaryReader reader) {
        ulong result = 0;
        var shift = 0;
        var bytesRead = 0;

        while (bytesRead < 10) {
            var b = reader.ReadByte();
            result |= (ulong)(b & 0x7F) << shift;
            bytesRead++;

            if ((b & 0x80) == 0) {
                return result;
            }

            shift += 7;
        }

        throw new FormatException("VarUInt64 did not terminate after 10 bytes.");
    }
}
