using System;
using System.Collections.Generic;

namespace uchebnayaPractica.Models;

public partial class City
{
    public int Id { get; set; }

    public string NameCity { get; set; } = null!;

    public string Hyperlink { get; set; } = null!;

    public virtual ICollection<Event> Idevent { get; set; } = new List<Event>();
}
