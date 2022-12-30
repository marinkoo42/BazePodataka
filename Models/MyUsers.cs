using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BazePodatakaProjekat.Models
{
    public class MyUsers
    {
        public string Id { get; set; } = String.Empty;

        public string Role { get; set; } = String.Empty;


        public string Token { get; set; } = String.Empty;

        public string ValidTo { get; set; } = String.Empty;

        [Required(ErrorMessage = "Polje ime je obavezno")]
        public string Name { get; set; } = String.Empty;

        [Required(ErrorMessage = "Polje prezime je obavezno")]
        public string LastName { get; set; } = String.Empty;

        [Required(ErrorMessage = "Polje username je obavezno")]
        public string UserName { get; set; } = String.Empty;

        //[Required(ErrorMessage = "Polje lozinka je obavezno!")]

        public string Password { get; set; } = String.Empty;

        [Required(ErrorMessage = "Polje broj telefona je obavezno")]

        public string Phone { get; set; } = String.Empty;

        [Required(ErrorMessage = "Polje email je obavezno!")]
        public string Email { get; set; } = String.Empty;

    }
}
