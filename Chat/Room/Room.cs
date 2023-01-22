namespace BazePodatakaProjekat.Chat.Room
{
    public class Room
    {
        public class ChatRoom 
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public IEnumerable<string>? Names { get; set; }
        }
    }
}
