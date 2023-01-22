using StackExchange.Redis;

namespace BazePodatakaProjekat.ChatHub
{

    public class RedisPublisher
    {
        private readonly IDatabase _db;
        private readonly ConnectionMultiplexer _redis;

        public RedisPublisher()
        {
            _redis = ConnectionMultiplexer.Connect("localhost:6379");
            _db = _redis.GetDatabase();
        }

        public void Publish(string channel, string message)
        {
            _db.Publish(channel, message);
        }
    }

}
