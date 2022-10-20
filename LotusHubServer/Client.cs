using System.Net.Sockets;
using LotusHubServer.Net.IO;

namespace LotusHubServer;

public class Client
{
    public string Username { get; set; }
    public Guid UID { get; set; }
    public TcpClient ClientSocket { get; set; }

    private PacketReader _packetReader;
    
    public Client(TcpClient client)
    {
        ClientSocket = client;
        UID = Guid.NewGuid();
        _packetReader = new PacketReader(ClientSocket.GetStream());

        var opcode = _packetReader.ReadByte();
        Username = _packetReader.ReadMessage();

        Console.WriteLine($"[{DateTime.Now}] Client has connected with username {Username}");
    }
}