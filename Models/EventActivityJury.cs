using System;
using System.Collections.Generic;

namespace uchebnayaPractica.Models;

public partial class EventActivityJury
{
    public int Id { get; set; }

    public int Idevent { get; set; }

    public int Idactivity { get; set; }

    public int IduserJury { get; set; }

    public virtual Activity IdactivityNavigation { get; set; } = null!;

    public virtual Event IdeventNavigation { get; set; } = null!;

    public virtual User IduserJuryNavigation { get; set; } = null!;
}
