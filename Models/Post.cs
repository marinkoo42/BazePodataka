using System.Diagnostics.Contracts;

namespace BazePodatakaProjekat.Models;

public class Post
{
    public Guid Id { get; set; }

    public int NumberOfLikes { get; set; }

    public string Url { get; set; } = String.Empty;

    public string Description { get; set; }  = String.Empty;

    public DateTime DateTimeCreated { get; set; }

}