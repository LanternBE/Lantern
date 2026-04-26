using BedrockProtocol.Types;
using Lantern.Protocol;
using RakSharp.Utils;
using PlayStatusPacket = BedrockProtocol.PlayStatus;

namespace Lantern.Handling.Handlers;

public class ResourcePackClientResponseHandler : BedrockPacketHandler<ResourcePackClientResponse> {

    private const ulong LocalPlayerRuntimeId = 1;
    private const int DefaultViewDistance = 10;

    public override async Task<bool> HandleAsync() {
        Logger.LogDebug($"Received ResourcePackClientResponse ({Packet.Status}) from ({ClientEndPoint})");

        switch (Packet.Status) {
            case ResourcePackClientResponse.StatusRefused:
                Logger.LogInfo($"Client ({ClientEndPoint}) refused resource packs.");
                return false;

            case ResourcePackClientResponse.StatusSendPacks:
                Logger.LogDebug($"Client ({ClientEndPoint}) requested packs, but server has no packs configured.");
                await SendBedrockPacketAsync(ResourcePackStack.Create(Compression.Algorithm.Zlib));
                return true;

            case ResourcePackClientResponse.StatusHaveAllPacks:
                await SendBedrockPacketAsync(ResourcePackStack.Create(Compression.Algorithm.Zlib));
                return true;

            case ResourcePackClientResponse.StatusCompleted:
                Logger.LogInfo($"Client ({ClientEndPoint}) completed resource pack handshake.");
                await SendGameplayStartSequenceAsync();
                return true;

            default:
                Logger.LogDebug($"Unhandled ResourcePackClientResponse status {Packet.Status} from ({ClientEndPoint}).");
                return false;
        }
    }

    private async Task SendGameplayStartSequenceAsync() {
        var worldName = string.IsNullOrWhiteSpace(Server.ServerInfo.ServerName) ? "Lantern" : Server.ServerInfo.ServerName;
        var viewDistance = DefaultViewDistance;

        await SendBedrockPacketAsync(StartGame.Create(
            compressionAlgorithm: Compression.Algorithm.Zlib,
            entityRuntimeId: LocalPlayerRuntimeId,
            worldName: worldName,
            playerGameMode: 0,
            x: 0,
            y: 64,
            z: 0
        ));

        await SendBedrockPacketAsync(BiomeDefinitionList.Create(Compression.Algorithm.Zlib));
        await SendBedrockPacketAsync(AvailableEntityIdentifiers.Create(Compression.Algorithm.Zlib));
        await SendBedrockPacketAsync(CreativeContent.Create(Compression.Algorithm.Zlib));
        await SendBedrockPacketAsync(ChunkRadiusUpdated.Create(Compression.Algorithm.Zlib, viewDistance));
        await SendBedrockPacketAsync(PlayStatusPacket.Create(Compression.Algorithm.Zlib, PlayStatus.PlayerSpawn));
    }
}
