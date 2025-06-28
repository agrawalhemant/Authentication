using System;
using System.Collections.Generic;

namespace Authentication.DAL.Models;

public partial class UserKyc
{
    public Guid id { get; set; }

    public Guid? userid { get; set; }

    public string? aadharnumber { get; set; }

    public string? pannumber { get; set; }

    public string? gstin { get; set; }

    public string? kycstatus { get; set; }

    public DateTime? verifiedat { get; set; }

    public DateTime? createdat { get; set; }

    public DateTime? updatedat { get; set; }

    public virtual User? user { get; set; }
}
