using BedrockProtocol;
using Lantern.Handling.Handlers;

namespace Lantern.Handling;

public class BedrockHandler {
    
    private readonly Dictionary<Type, Type> _packetHandlers = new();

    public void InitializeDefaultHandlers() {
        
        RegisterHandler<RequestNetworkSettings, RequestNetworkSettingsHandler>();
    }

    public void RegisterHandler<TPacket, THandler>() where THandler : class {
        _packetHandlers[typeof(TPacket)] = typeof(THandler);
    }

    public Type? GetHandlerType(Type packetType) {
        return _packetHandlers.TryGetValue(packetType, out var handlerType) ? handlerType : null;
    }

    public object CreateHandler(Type handlerType) {
        return Activator.CreateInstance(handlerType);
    }
}