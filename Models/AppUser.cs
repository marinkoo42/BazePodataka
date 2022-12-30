//using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Neo4j.AspNet.Identity;

namespace BazePodatakaProjekat.Models
{
    public class AppUser : IdentityUser
    {


        [Required(ErrorMessage = "Polje ime je obavezno")]
        public string Name { get; set; } = String.Empty;


        [Required(ErrorMessage = "Polje prezime je obavezno")]
        public string LastName { get; set; } = String.Empty;

    }
}
