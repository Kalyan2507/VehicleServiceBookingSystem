using System.ComponentModel.DataAnnotations;

namespace VehicleServiceBook.Models.DTOS
{
    public class LoginDto
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? PasswordHash { get; set; }
    }
}
