using BazePodatakaProjekat.Chat.Room;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;
using StackExchange.Redis;

namespace BazePodatakaProjekat.ChatHub
{
    [SignalRHub]

    public class ChatHub : Hub
    {
        /* private readonly string _botUser;
         private readonly IDictionary<string,UserConnection> _connections;*//*

         private readonly IConnectionMultiplexer _connections;
         public ChatHub(IConnectionMultiplexer connections){

             _connections = connections;
         }


         public async Task SendMessage([FromBody]RoomMessage message)
         {
             //Context.ConnectionId

             await Clients.All.SendAsync("ReceiveMessage", message);


         }


         public override async Task OnConnectedAsync()
         {
             await Groups.AddToGroupAsync(Context.ConnectionId, "HubUsers");
             await base.OnConnectedAsync();
         }

         public override async Task OnDisconnectedAsync(Exception? exception)
         {
             await Groups.RemoveFromGroupAsync(Context.ConnectionId, "HubUsers");
             await base.OnDisconnectedAsync(exception);
         }
         public async Task JoinRoom(string roomId, string userId)//UserConnection userConnection)
         {

             await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
             //_connections[Context.ConnectionId] = userId;
         }
             */
        /*public override Task OnDisconnectedAsync(Exception exception)
                {
                    if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
                    {
                        _connections.Remove(Context.ConnectionId);
                        Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser,
                        $"{userConnection.User} has left");

                        SendConnectedUsers(userConnection.Room);
                    }
                    return base.OnDisconnectedAsync(exception);
                }*/

        private readonly RedisSubscriber _subscriber;

        public ChatHub(RedisSubscriber subscriber)
        {
            _subscriber = subscriber;
        }

        public async Task Subscribe(string channel)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channel);
            _subscriber.Subscribe(channel, (c, m) => Clients.Group(channel).SendAsync("message", m));
        }

        public async Task Unsubscribe(string channel)
        {
            _subscriber.Unsubscribe(channel);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channel);
        }

        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task SubscribeRoom(string roomName)
        {
            _subscriber.Subscribe(roomName, (c, m) => Clients.Group(roomName).SendAsync("message", m));
        }


        /*public async Task SendMessage(string message, string roomName)
        {
            await Clients.Group(roomName).SendAsync("message", message);
        }
*/

        public async Task SendMessage(string message, string roomName)
        {
            await Clients.Group(roomName).SendAsync("message", message);
            IDatabase db = redis.GetDatabase();
            string key = $"{roomName}:{DateTime.UtcNow.Ticks}";
            bool success = db.StringSet(key, message);
        }



}
}
