using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using LotusHub.MVVM.Core;
using LotusHub.MVVM.Model;
using LotusHub.Net;

namespace LotusHub.MVVM.ViewModel;

public class MainViewModel
{
    public ObservableCollection<UserModel> Users { get; set; }
    public ObservableCollection<string> Messages { get; set; }
    public RelayCommand ConnectToServerCommand { get; set; }
    public RelayCommand SendMessageCommand { get; set; }

    public string Username { get; set; }
    public string Message { get; set; }

    private Server _server;

    public MainViewModel()
    {
        Users = new ObservableCollection<UserModel>();
        Messages = new ObservableCollection<string>();
        _server = new Server();
        _server.connectedEvent += UserConnected;
        _server.msgReceivedEvent += MessageReceived;
        _server.userDisconnectedEvent += UserDisconnected;
        ConnectToServerCommand =
            new RelayCommand(_ => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));

        SendMessageCommand =
            new RelayCommand(_ => _server.ConnectToServer(Username), _ => !string.IsNullOrEmpty(Message));
    }

    private void UserDisconnected()
    {
        var uid = _server.PacketReader.ReadMessage();
        var user = Users.FirstOrDefault(x => x.UID == uid);
        Application.Current.Dispatcher.Invoke(() => { Users.Remove(user); });
    }

    private void MessageReceived()
    {
        var msg = _server.PacketReader.ReadMessage();
        Application.Current.Dispatcher.Invoke(() => { Messages.Add(msg); });
    }

    private void UserConnected()
    {
        var user = new UserModel
        {
            Username = _server.PacketReader.ReadMessage(),
            UID = _server.PacketReader.ReadMessage()
        };

        if (Users.All(x => x.UID != user.UID))
        {
            Application.Current.Dispatcher.Invoke(() => { Users.Add(user); });
        }
    }
}