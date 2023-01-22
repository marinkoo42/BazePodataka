
using System.ComponentModel.DataAnnotations;
using AspNetCore.Identity.Neo4j;


namespace BazePodatakaProjekat.Models
{
    public class AppUser : Neo4jIdentityUser
    {

        [Required(ErrorMessage = "Polje ime je obavezno")]
        public string Name { get; set; } = String.Empty;


        [Required(ErrorMessage = "Polje prezime je obavezno")]
        public string LastName { get; set; } = String.Empty;

        public string ProfilePicture { get; set; } = String.Empty;

        public string ProfileDescription { get; set; } = String.Empty;

        public int NumbersOfFollowers { get; set; } = 0;

        public int NumbersOfFollowings { get; set; } = 0;
    }
}
