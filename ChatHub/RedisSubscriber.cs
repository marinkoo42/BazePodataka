using StackExchange.Redis;

namespace BazePodatakaProjekat.ChatHub
{
    public class RedisSubscriber
    {
        private readonly ISubscriber _sub;
        private readonly IConnectionMultiplexer _redis;

        public RedisSubscriber(IConnectionMultiplexer redis)
        {
            _redis = redis;//ConnectionMultiplexer.Connect("localhost:6379");
            _sub = _redis.GetSubscriber();
        }

        public void Subscribe(string channel, Action<RedisChannel, RedisValue> handler)
        {
            _sub.Subscribe(channel, handler);
        }

        public void Unsubscribe(string channel)
        {
            _sub.Unsubscribe(channel);
        }
    }
}
