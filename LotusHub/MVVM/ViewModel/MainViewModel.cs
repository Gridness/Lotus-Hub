using System;
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
    public RelayCommand ConnectToServerCommand { get; set; }
    public RelayCommand SendMessageCommand { get; set; }

    public string Username { get; set; }
    public string Message { get; set; }

    private Server _server;

    public MainViewModel()
    {
        Users = new ObservableCollection<UserModel>();
        _server = new Server();
        _server.connectedEvent += UserConnected;
        ConnectToServerCommand =
            new RelayCommand(_ => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                Users.Add(user);
            });
        }
    }
}