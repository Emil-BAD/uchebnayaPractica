using System;
using System.Collections.Generic;

namespace uchebnayaPractica.Models;

public partial class Activity
{
    public int Id { get; set; }

    public string NameActivity { get; set; } = null!;

    public virtual ICollection<ActivityEvent> ActivityEvent { get; set; } = new List<ActivityEvent>();

    public virtual ICollection<EventActivityJury> EventActivityJury { get; set; } = new List<EventActivityJury>();
}
