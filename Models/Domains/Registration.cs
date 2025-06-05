using System;
using System.Collections.Generic;

namespace VehicleServiceBook.Models.Domains;

public partial class Registration
{
    public int UserId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? PasswordHash { get; set; }

    public string? Role { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<ServiceCenter> ServiceCenters { get; set; } = new List<ServiceCenter>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
