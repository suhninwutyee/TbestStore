using System.ComponentModel.DataAnnotations;

namespace MVCtesting.Models
{
    public class ApplicationSetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Site Name")]
        public string SiteName { get; set; }

        [Display(Name = "Maintenance Mode")]
        public bool IsMaintenanceMode { get; set; }

        [Display(Name = "Support Email")]
        public string SupportEmail { get; set; }
        [Display(Name = "Administrator Email")]
        [EmailAddress]
        public string AdministratorEmail { get; set; }

    }
}
