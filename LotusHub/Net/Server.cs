using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using LotusHub.Net.IO;

namespace LotusHub.Net;

public class Server
{
    private const string IP = "127.0.0.1";
    private const int PORT = 7891;
    
    private TcpClient _client;
    public PacketReader PacketReader;

    public event Action connectedEvent;

    public Server()
    {
        _client = new TcpClient();
    }

    public void ConnectToServer(string username)
    {
        if (!_client.Connected)
        {
            _client.Connect(IP, PORT);
            PacketReader = new PacketReader(_client.GetStream());

            if (!string.IsNullOrEmpty(username))
            {
                var connectPacket = new PacketBuilder();
                connectPacket.WriteOpCode(0);
                connectPacket.WriteMessage(username);
                _client.Client.Send(connectPacket.GetPacketBytes());
            }

            ReadPackets();
        }
    }

    private void ReadPackets()
    {
        Task.Run(() =>
        {
            while (true)
            {
                var opcode = PacketReader.ReadByte();
                switch (opcode)
                {
                    case 1:
                        connectedEvent?.Invoke();
                        break;
                    default:
                        Console.WriteLine("ah yes...");
                        break;
                }
            }
        });
    }

    public void SendMessageToServer(string message)
    {
        var messagePacket = new PacketBuilder();
        messagePacket.WriteOpCode(5);
        messagePacket.WriteMessage(message);
        _client.Client.Send(messagePacket.GetPacketBytes());
    }
}