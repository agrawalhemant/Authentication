using System;
using System.Collections.Generic;

namespace Authentication.DAL.Models;

public partial class EmailVerification
{
    public Guid id { get; set; }

    public Guid? userid { get; set; }

    public string verificationtoken { get; set; } = null!;

    public DateTime expiresat { get; set; }

    public bool isused { get; set; } = false;

    public DateTime createdat { get; set; } = DateTime.UtcNow;

    public virtual User? user { get; set; }
}
