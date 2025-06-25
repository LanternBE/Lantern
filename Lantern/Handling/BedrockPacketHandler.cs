using System.Net;
using System.Net.Sockets;
using RakSharp;

namespace Lantern.Handling;

public abstract class BedrockPacketHandler<T> {
    
    protected Server Server { get; set; }
    protected Socket Socket { get; set; }
    protected IPEndPoint ClientEndPoint { get; set; }
    protected byte[] Buffer { get; set; }
    protected T Packet { get; set; }

    public void Initialize(Server server, Socket socket, IPEndPoint clientEndPoint, byte[] buffer, T packet) {
        
        Server = server;
        Socket = socket;
        ClientEndPoint = clientEndPoint;
        Buffer = buffer;
        Packet = packet;
    }

    public abstract Task<bool> HandleAsync();
}