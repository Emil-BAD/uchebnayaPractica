using System;
using System.Collections.Generic;

namespace uchebnayaPractica.Models;

public partial class ActivityEvent
{
    public int Idactivity { get; set; }

    public int Idevent { get; set; }

    public int Day { get; set; }

    public TimeOnly TimeStart { get; set; }

    public int IduserModer { get; set; }

    public virtual Activity IdactivityNavigation { get; set; } = null!;

    public virtual Event IdeventNavigation { get; set; } = null!;

    public virtual User IduserModerNavigation { get; set; } = null!;
}
