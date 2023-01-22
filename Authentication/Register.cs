using System.ComponentModel.DataAnnotations;

namespace BazePodatakaProjekat.Authentication
{
    public class Register
    {

        [Required(ErrorMessage = "Polje ime je obavezno")]
        public string Name { get; set; } = String.Empty;

        [Required(ErrorMessage = "Polje prezime je obavezno")]
        public string LastName { get; set; } = String.Empty;

        [Required(ErrorMessage = "Polje username je obavezno")]
        public string UserName { get; set; } = String.Empty;

        [Required(ErrorMessage = "Polje lozinka je obavezno!")]

        public string Password { get; set; } = String.Empty;

        [Required(ErrorMessage = "Polje broj telefona je obavezno")]
        [StringLength(13, MinimumLength = 10)]
        [Phone]
        public string Phone { get; set; } = String.Empty;

        [Required(ErrorMessage = "Polje email je obavezno!")]
        [EmailAddress]
        public string Email { get; set; } = String.Empty;
    }
}
