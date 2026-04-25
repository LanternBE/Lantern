using System.Net;
using System.Net.Sockets;
using BedrockProtocol.Types;
using RakSharp;

namespace Lantern.Handling.Interfaces;

public interface IBedrockPacketHandler {
    void Initialize(Server server, Socket socket, IPEndPoint clientEndPoint, byte[] buffer, BedrockPacket packet);
    Task<bool> HandleAsync();
}
