﻿namespace VehicleServiceBook.Models.DTOS
{
    public class UserDto
    {
        public int UserId { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        //public string? PasswordHash { get; set; }

        //public string? Role { get; set; }
    }
}
