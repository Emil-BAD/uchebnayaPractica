using System;
using System.Collections.Generic;

namespace uchebnayaPractica.Models;

public partial class TypeofEvent
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<EventTypeDictionary> EventTypeDictionary { get; set; } = new List<EventTypeDictionary>();
}
