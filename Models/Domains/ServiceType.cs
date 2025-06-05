using System;
using System.Collections.Generic;

namespace VehicleServiceBook.Models.Domains;

public partial class ServiceType
{
    public int ServiceTypeId { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
