using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;

        public ChatHub(IDictionary<string, UserConnection> connections)
        {
            _botUser = "MyChat Bot";
            _connections = connections;
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            // Adiciona o usuário no dicionário de conexões
            _connections[Context.ConnectionId] = userConnection;

            // Cria um grupo com o nome da sala.
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser,
                $"{userConnection.User} has joined {userConnection.Room}");

            await SendConnectedUser(userConnection.Room);

            // Se não adicionar um grupo manda para todos que estiver conectados na aplicaçao
            //await Clients.All.SendAsync("ReceiveMessage", _botUser,
            //    $"{userConnection.User} has joined {userConnection.Room}");
        }

        public async Task SendMessage(string message)
        {
            if(_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", userConnection.User, message);
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if(_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has left");

                SendConnectedUser(userConnection.Room);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public Task SendConnectedUser(string room)
        {
            var users = _connections.Values
                .Where(c => c.Room == room)
                .Select(c => c.User);

            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }
    }
}