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

    public DateTime? Date { get; set; }

    public string? TimeSlot { get; set; }

    public string? Status { get; set; }

    public virtual Invoice? Invoice { get; set; }

    public virtual ServiceCenter? ServiceCenter { get; set; }


    public virtual Vehicle? Vehicle { get; set; }
    public virtual ServiceType? ServiceType { get; set; }
}
