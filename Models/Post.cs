namespace BazePodatakaProjekat.Models;

public class Post
{
    public Guid Id { get; set; }

    public int NumberOfLikes { get; set; }

    public string Description { get; set; }  = String.Empty;


}