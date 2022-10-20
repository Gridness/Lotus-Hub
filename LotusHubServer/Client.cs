using System.Net.Sockets;
using LotusHubServer.Net.IO;

namespace LotusHubServer;

public class Client
{
    public string Username { get; set; }
    public Guid UID { get; set; }
    public TcpClient ClientSocket { get; set; }

    private readonly PacketReader _packetReader;

    public Client(TcpClient client)
    {
        ClientSocket = client;
        UID = Guid.NewGuid();
        _packetReader = new PacketReader(ClientSocket.GetStream());

        var opcode = _packetReader.ReadByte();
        Username = _packetReader.ReadMessage();

        Console.WriteLine($"[{DateTime.Now}] Client has connected with username {Username}");

        Task.Run(Process);
    }

    void Process()
    {
        while (true)
        {
            try
            {
                var opcode = _packetReader.ReadByte();
                switch (opcode)
                {
                    case 5:
                        var msg = _packetReader.ReadMessage();
                        Console.WriteLine($"[{DateTime.Now}] Message received: {msg}");
                        Program.BroadcastMessage($"[{DateTime.Now}] {Username}: {msg}");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"[{DateTime.Now}] {UID} has been disconnected");
                Program.BroadcastDisconnect(UID.ToString());
                ClientSocket.Close();
            }
        }
    }
}