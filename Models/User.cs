
using System.Xml.Linq;

namespace BazePodatakaProjekat.Models;


public class User
{


    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public List<User>? Following { get; set; }

    public List<User>? Followers { get; set; }

    public List<Post>? LikedPosts { get; set; }

    public List<Post>? Posts { get; set; }

    //public List<Comment> Comments { get; set; }



}

