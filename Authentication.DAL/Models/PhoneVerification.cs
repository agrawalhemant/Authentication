using System;
using System.Collections.Generic;

namespace Authentication.DAL.Models;

public partial class PhoneVerification
{
    public Guid id { get; set; }

    public Guid? userid { get; set; }

    public string otp { get; set; } = null!;

    public DateTime expiresat { get; set; }

    public bool isused { get; set; }

    public DateTime? createdat { get; set; }

    public virtual User? user { get; set; }
}
