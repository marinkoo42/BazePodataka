using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BazePodatakaProjekat.Authentication
{
    public class Login
    {
        [Required(ErrorMessage = "Polje username je obavezno")]
        public String UserName { get; set; } = String.Empty;

        [Required(ErrorMessage = "Polje lozinka je obavezno!")]
        public string Password { get; set; } = String.Empty;
    }
}
