using BedrockProtocol;
using BedrockProtocol.Types;
using Lantern.Handling.Handlers;
using Lantern.Handling.Interfaces;
using Lantern.Protocol;

namespace Lantern.Handling;

public class BedrockHandler {
    
    private readonly Dictionary<Type, Func<IBedrockPacketHandler>> _packetHandlers = new();

    public void InitializeDefaultHandlers() {
        
        RegisterHandler<RequestNetworkSettings, RequestNetworkSettingsHandler>();
        RegisterHandler<Login, LoginHandler>();
    }

    public void RegisterHandler<TPacket, THandler>()
        where TPacket : BedrockPacket
        where THandler : class, IBedrockPacketHandler, new() {
        _packetHandlers[typeof(TPacket)] = () => new THandler();
    }

    public IBedrockPacketHandler? CreateHandler(Type packetType) {
        return _packetHandlers.TryGetValue(packetType, out var handlerFactory) ? handlerFactory() : null;
    }
}
