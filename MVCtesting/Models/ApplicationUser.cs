using Microsoft.AspNetCore.Identity;

namespace MVCtesting.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }

    }

}
