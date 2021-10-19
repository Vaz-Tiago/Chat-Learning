using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs
{
    public class ChatHub : Hub
    {
                private readonly string _botUser;

        public ChatHub()
        {
            _botUser = "MyChat Bot";
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            // Cria um grupo com o nome da sala.
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser,
                $"{userConnection.User} has joined {userConnection.Room}");

            // Se não adicionar um grupo manda para todos que estiver conectados na aplicaçao
            //await Clients.All.SendAsync("ReceiveMessage", _botUser,
            //    $"{userConnection.User} has joined {userConnection.Room}");
        }
    }
}