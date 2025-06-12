using System;
using System.Collections.Generic;

namespace VehicleServiceBook.Models.Domains;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int? BookingId { get; set; }

    public int? ServiceTypeId { get; set; }

    public double? TotalAmount { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime Date { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ServiceType? ServiceType { get; set; }
}
