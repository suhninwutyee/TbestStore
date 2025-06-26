using System.ComponentModel.DataAnnotations;


    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    namespace MVCtesting.Models
    {
        public class EditUserProfileViewModel
        {
            public int Id { get; set; }

            [Required]
            [StringLength(100)]
            public string UserName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [StringLength(500)]
            public string? ProfileImageUrl { get; set; }

            // File upload for new profile picture
            public IFormFile? ProfileImage { get; set; }
        }
    }


