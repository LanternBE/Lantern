using BedrockProtocol.Types;
using Lantern.Protocol;
using RakSharp.Utils;

namespace Lantern.Handling.Handlers;

public class ResourcePackClientResponseHandler : BedrockPacketHandler<ResourcePackClientResponse> {

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
                return true;

            default:
                Logger.LogDebug($"Unhandled ResourcePackClientResponse status {Packet.Status} from ({ClientEndPoint}).");
                return false;
        }
    }
}
