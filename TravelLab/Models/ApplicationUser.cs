using Microsoft.AspNetCore.Identity;

namespace TravelLab.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
    }
}