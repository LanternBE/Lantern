using BedrockProtocol.Types;
using Lantern.Protocol;
using RakSharp.Utils;

namespace Lantern.Handling.Handlers;

public class RequestChunkRadiusHandler : BedrockPacketHandler<RequestChunkRadius> {

    public override async Task<bool> HandleAsync() {
        var requestedRadius = Math.Max(1, Packet.ChunkRadius);
        var maxServerRadius = Math.Max(1, Server.ConfigManager.Settings.ViewDistance);
        var negotiatedRadius = Math.Min(requestedRadius, maxServerRadius);

        Logger.LogDebug(
            $"Received RequestChunkRadius from ({ClientEndPoint}) requested={Packet.ChunkRadius}, max={Packet.MaxChunkRadius}, negotiated={negotiatedRadius}"
        );

        await SendBedrockPacketAsync(ChunkRadiusUpdated.Create(Compression.Algorithm.Zlib, negotiatedRadius));
        return true;
    }
}
