using Lantern.Protocol;
using RakSharp.Utils;

namespace Lantern.Handling.Handlers;

public class SetLocalPlayerAsInitializedHandler : BedrockPacketHandler<SetLocalPlayerAsInitialized> {

    public override Task<bool> HandleAsync() {
        Logger.LogInfo(
            $"Client ({ClientEndPoint}) is fully initialized with runtime entity id {Packet.EntityRuntimeId}."
        );
        return Task.FromResult(true);
    }
}
