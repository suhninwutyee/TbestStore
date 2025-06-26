using System.ComponentModel.DataAnnotations;

namespace MVCtesting.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public bool IsActive { get; set; }
        
    }
}
