using Newtonsoft.Json;
using StackExchange.Redis;
using static BazePodatakaProjekat.Chat.Room.Room;
using BazePodatakaProjekat.Chat.Room;
using Microsoft.AspNetCore.Mvc;

namespace BazePodatakaProjekat.Chat.ChatService
{
    public class ChatService
    {

        private readonly IConnectionMultiplexer _redis;
        private IDatabase _database;

        public ChatService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
        }



        public async Task<List<RoomMessage>> GetMessages(string room, int offset = 0, int size = 50)//(string roomId = "0", int offset = 0, int size = 50)
        {
            


            //var roomKey = $"room:{pubId}:{subId}";
            var roomExists = await _database.KeyExistsAsync(room);
            var messages = new List<RoomMessage>();

            if (!roomExists)
            {
                return messages;
            }
            else
            {
                var values = await _database.SortedSetRangeByRankAsync(room, offset, offset + size, Order.Ascending);

                foreach (var valueRedisVal in values)
                {
                    var value = valueRedisVal.ToString();
                    try
                    {
                        messages.Add(JsonConvert.DeserializeObject<RoomMessage>(value));
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        // Console.WriteLine($"Couldn't deserialize json: {value}");
                    }
                }
                return messages;
            }
        }

        public async Task<List<ChatRoom>> GetRooms(int userId = 0)
        {
            var roomIds = await _database.SetMembersAsync($"user:{userId}:rooms");
            var rooms = new List<ChatRoom>();
            foreach (var roomIdRedisValue in roomIds)
            {
                var roomId = roomIdRedisValue.ToString();
                var name = await _database.StringGetAsync($"room:{roomId}:name");
                if (name.IsNullOrEmpty)
                {
                    // It's a room without a name, likey the one with private messages 
                    var roomExists = await _database.KeyExistsAsync($"room:{roomId}");
                    if (!roomExists)
                    {
                        continue;
                    }

                    var userIds = roomId.Split(':');
                    if (userIds.Length != 2)
                    {
                        throw new Exception("You don't have access to this room");
                    }

                    rooms.Add(new ChatRoom()
                    {
                        Id = roomId,
                        Names = new List<string>() {
                            (await _database.HashGetAsync($"user:{userIds[0]}", "username")).ToString(),
                            (await _database.HashGetAsync($"user:{userIds[1]}", "username")).ToString(),
                        }
                    });
                }
                else
                {
                    rooms.Add(new ChatRoom()
                    {
                        Id = roomId,
                        Names = new List<string>() {
                            name.ToString()
                        }
                    });
                }
            }
            return rooms;
        }

        public async Task SendMessage(string room, RoomMessage message)
        {
            var roomKey = room;
            await _database.SortedSetAddAsync(roomKey, JsonConvert.SerializeObject(message), (double)message.Date);
            
            //await PublishMessage("message", message);
        }


    }
}
