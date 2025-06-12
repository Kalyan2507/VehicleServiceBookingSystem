using System;
using System.Collections.Generic;

namespace VehicleServiceBook.Models.Domains;

public partial class Booking
{
    public virtual Registration Registration { get; set; }

    public int Bookingid { get; set; }

    public int? UserId { get; set; }

    public int? VehicleId { get; set; }

    public int? ServiceCenterId { get; set; }
    public int ServiceTypeId { get; set; }
    public int? MechanicId { get; set; }
    public DateTime Date { get; set; }

    public string TimeSlot { get; set; } = null!;

    public string? Status { get; set; }
    public Registration Registration { get; set; }
    public Vehicle Vehicle { get; set; }
    public ServiceCenter ServiceCenter { get; set; }
    public ServiceType ServiceType { get; set; }
    public Invoice Invoice { get; set; }
    public Mechanic Mechanic { get; set; }
}
