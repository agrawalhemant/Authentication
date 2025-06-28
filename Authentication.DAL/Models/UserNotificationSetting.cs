using System;
using System.Collections.Generic;

namespace Authentication.DAL.Models;

public partial class UserNotificationSetting
{
    public Guid id { get; set; }

    public Guid? userid { get; set; }

    public bool? marketingemailsenabled { get; set; }

    public bool? productupdatesenabled { get; set; }

    public bool? smsalertsenabled { get; set; }

    public DateTime? createdat { get; set; }

    public DateTime? updatedat { get; set; }

    public virtual User? user { get; set; }
}
