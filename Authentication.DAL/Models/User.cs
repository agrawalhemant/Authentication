using System;
using System.Collections.Generic;

namespace Authentication.DAL.Models;

public partial class User
{
    public Guid id { get; set; }

    public string firstname { get; set; } = null!;

    public string? lastname { get; set; }

    public string email { get; set; } = null!;

    public string? phonenumber { get; set; }

    public string passwordhash { get; set; } = null!;

    public bool isemailverified { get; set; }

    public bool isphoneverified { get; set; }

    public string preferredlanguage { get; set; }

    public string role { get; set; }

    public bool isactive { get; set; } = true;

    public DateTime createdat { get; set; } = DateTime.UtcNow;

    public DateTime updatedat { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Address> addresses { get; set; } = new List<Address>();

    public virtual ICollection<EmailVerification> emailverifications { get; set; } = new List<EmailVerification>();

    public virtual ICollection<PhoneVerification> phoneverifications { get; set; } = new List<PhoneVerification>();

    public virtual ICollection<UserKyc> userkycs { get; set; } = new List<UserKyc>();

    public virtual ICollection<UserNotificationSetting> usernotificationsettings { get; set; } = new List<UserNotificationSetting>();
}