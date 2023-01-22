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



        [Route("getMessages/{pubId}/{subId}")]
        [HttpGet]
        public async Task<List<RoomMessage>> GetMessages(string pubId,string subId)
        {
            return await _service.GetMessages(pubId, subId);
        }
            
    }
}
