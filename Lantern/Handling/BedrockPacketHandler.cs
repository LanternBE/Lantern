using System.Net;
using System.Net.Sockets;
using BedrockProtocol;
using BedrockProtocol.Types;
using Lantern.Handling.Interfaces;
using RakSharp;
using RakSharp.Protocol;
using RakSharp.Protocol.Online;
using RakSharp.Utils;

namespace Lantern.Handling;

public abstract class BedrockPacketHandler<T> : IBedrockPacketHandler where T : BedrockPacket {
    
    protected Server Server { get; private set; } = null!;
    protected Socket Socket { get; private set; } = null!;
    protected IPEndPoint ClientEndPoint { get; private set; } = null!;
    protected byte[] Buffer { get; private set; } = null!;
    protected T Packet { get; private set; } = null!;

    public void Initialize(Server server, Socket socket, IPEndPoint clientEndPoint, byte[] buffer, BedrockPacket packet) {
        
        Server = server;
        Socket = socket;
        ClientEndPoint = clientEndPoint;
        Buffer = buffer;
        
        if (packet is not T typedPacket) {
            throw new InvalidOperationException($"Invalid packet type. Expected {typeof(T).Name}, received {packet.GetType().Name}");
        }
        
        Packet = typedPacket;
    }

    public abstract Task<bool> HandleAsync();
    
    protected async Task SendEncapsulatedPacketAsync((EncapsulatedPacket packet, byte[] buffer) response) {
        
        var session = Server.SessionsManager.GetSession(ClientEndPoint);
        if (session is null) {
            Logger.LogError($"Session not found for the client endpoint ({ClientEndPoint})");
            return;
        }
        
        var datagram = Datagram.Create(0, [response.packet], session.GetNextSequenceNumber());
        await Socket.SendToAsync(datagram.buffer, SocketFlags.None, ClientEndPoint);
    }
    
    protected async Task SendBedrockPacketAsync((BedrockPacket packet, byte[] buffer) response) {

        var tempBuffer = new byte[1492];
        var writer = new RakSharp.Binary.BinaryWriter(tempBuffer);
        
        response.packet.Write(writer);
        var bedrockBuffer = new byte[writer.Position];
        
        Array.Copy(tempBuffer, bedrockBuffer, writer.Position);
        var (gamePacket, gamePacketBuffer) = GamePacket.Create(
            subClientId: 0,
            subTargetId: 0,
            payload: bedrockBuffer
        );
        
        Console.WriteLine($"Buffer gamepacket: {BitConverter.ToString(gamePacketBuffer)}");
        
        var session = Server.SessionsManager.GetSession(ClientEndPoint);
        if (session == null)
            throw new InvalidOperationException($"Session not found for {ClientEndPoint}");
        
        var encapsulatedPacket = EncapsulatedPacket.Create(gamePacketBuffer, PacketReliability.ReliableOrdered, session.GetNextReliableIndex(), session.GetNextOrderedIndex());
        Console.WriteLine($"Buffer encapsulatedpacket: {BitConverter.ToString(encapsulatedPacket.ToBytes())}");
        
        var sequenceNumber = session.GetNextSequenceNumber();
        var datagram = Datagram.Create(0, [encapsulatedPacket], sequenceNumber);
        
        Console.WriteLine($"Buffer datagram: {BitConverter.ToString(datagram.buffer)}");
        session.TrackReliablePacket(sequenceNumber, datagram);
        
        await Socket.SendToAsync(datagram.buffer, SocketFlags.None, ClientEndPoint);
    }
}
