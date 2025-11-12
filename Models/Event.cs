using System;
using System.Collections.Generic;

namespace uchebnayaPractica.Models;

public partial class Event
{
    public int Id { get; set; }

    public string NameEvent { get; set; } = null!;

    public DateOnly DateStart { get; set; }

    public int Days { get; set; }

    public int? Iduser { get; set; }

    public virtual ICollection<ActivityEvent> ActivityEvent { get; set; } = new List<ActivityEvent>();

    public virtual ICollection<EventActivityJury> EventActivityJury { get; set; } = new List<EventActivityJury>();

    public virtual ICollection<EventTypeDictionary> EventTypeDictionary { get; set; } = new List<EventTypeDictionary>();

    public virtual User? IduserNavigation { get; set; }

    public virtual ICollection<City> Idcity { get; set; } = new List<City>();

    public virtual ICollection<User> Iduser1 { get; set; } = new List<User>();
}
