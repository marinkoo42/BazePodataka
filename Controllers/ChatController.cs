using BazePodatakaProjekat.Chat.ChatService;
using BazePodatakaProjekat.Chat.Room;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace BazePodatakaProjekat.Controllers
{




    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {


        ChatService _service;

        public ChatController(IConnectionMultiplexer redis) {
            _service = new ChatService(redis);
        }



        [Route("getMessages/{room}")]
        [HttpGet]
        public async Task<List<RoomMessage>> GetMessages(string room)
        {
            return await _service.GetMessages(room);
        }

        [Route("sendMessage/{room}")]
        [HttpPost]
        public async Task SendMessage(string room , [FromBody] RoomMessage message)
        {
            await _service.SendMessage(room,message);
        }

    }
}
