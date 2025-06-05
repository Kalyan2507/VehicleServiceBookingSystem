namespace VehicleServiceBook.Models.DTOS
{
    public class RegisterUserDto
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? PasswordHash { get; set; }

        public string? Role { get; set; }
    }
}
