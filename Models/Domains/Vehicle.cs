using System;
using System.Collections.Generic;

namespace VehicleServiceBook.Models.Domains;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public int? UserId { get; set; }

    public string Make { get; set; } = null!;

    public string? Model { get; set; }

    public int? Year { get; set; }

    public string? RegistrationNumber { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } 

    public virtual Registration? User { get; set; }
}
