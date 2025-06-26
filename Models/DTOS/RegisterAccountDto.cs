using System.ComponentModel.DataAnnotations;
namespace VehicleServiceBook.Models.DTOS
{
    public class RegisterAccountDto
    {
        [Required]
        public string? Role { get; set; } // "User" or "ServiceCenter"

        [Required]
        public string? Name { get; set; }

        [Required,EmailAddress]
        public string? Email { get; set; }

        [Required,RegularExpression(@"^\d{10}$",ErrorMessage ="Phone must be exactly 10 digits")]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Password { get; set; } // Plain password

        // Optional for ServiceCenter
        public string? ServiceCenterName { get; set; }
        public string? ServiceCenterLocation { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "ServiceCenterContact must be exactly 10 digits")]
        public string? ServiceCenterContact { get; set; }
    }
}
