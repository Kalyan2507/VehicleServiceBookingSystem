using System;
using System.Collections.Generic;

namespace VehicleServiceBook.Models.Domains;

public partial class ServiceCenter
{
    public int ServiceCenterId { get; set; }

    public int? UserId { get; set; }

    public string ServiceCenterName { get; set; } = null!;

    public string? ServiceCenterLocation { get; set; }

    public string? ServiceCenterContact { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Mechanic> Mechanics { get; set; } = new List<Mechanic>();

    public virtual Registration? User { get; set; }
}
