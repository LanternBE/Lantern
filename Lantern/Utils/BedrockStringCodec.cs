using System.Text;
using BinaryReader = RakSharp.Binary.BinaryReader;
using BinaryWriter = RakSharp.Binary.BinaryWriter;

namespace Lantern.Utils;

public static class BedrockStringCodec {

    public static string Read(BinaryReader reader) {
        var length = (int)reader.ReadVarUInt();
        if (length == 0) {
            return string.Empty;
        }

        var bytes = reader.ReadBytes(length);
        return Encoding.UTF8.GetString(bytes);
    }

    public static void Write(BinaryWriter writer, string value) {
        var bytes = Encoding.UTF8.GetBytes(value);
        writer.WriteVarUInt((uint)bytes.Length);
        writer.Write(bytes);
    }
}
