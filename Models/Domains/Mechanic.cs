using System;
using System.Collections.Generic;

namespace VehicleServiceBook.Models.Domains;

public partial class Mechanic
{
    public int Mechanicid { get; set; }

    public int? ServiceCenterId { get; set; }

    public string? MechanicName { get; set; }

    public string? Expertise { get; set; }

    public virtual ServiceCenter? ServiceCenter { get; set; }
}
