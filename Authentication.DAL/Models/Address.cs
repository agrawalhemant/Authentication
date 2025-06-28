using System;
using System.Collections.Generic;

namespace Authentication.DAL.Models;

public partial class Address
{
    public Guid id { get; set; }

    public Guid? userid { get; set; }

    public string? addresstype { get; set; }

    public string streetaddress { get; set; } = null!;

    public string? locality { get; set; }

    public string? district { get; set; }

    public string? city { get; set; }

    public string? state { get; set; }

    public string? pincode { get; set; }

    public string? landmark { get; set; }

    public decimal? latitude { get; set; }

    public decimal? longitude { get; set; }

    public DateTime? createdat { get; set; }

    public DateTime? updatedat { get; set; }

    public virtual User? user { get; set; }
}
