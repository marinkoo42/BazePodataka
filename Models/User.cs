
using System.Xml.Linq;

namespace BazePodatakaProjekat.Models;


public class User
{

    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string ProfilePicture { get; set; } = String.Empty;

    public string ProfileDescription { get; set; } = String.Empty;

    public int NumbersOfFollowers { get; set; } = 0;

    public int NumbersOfFollowings { get; set; } = 0;





}

