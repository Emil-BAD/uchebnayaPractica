using System;
using System.Collections.Generic;

namespace uchebnayaPractica.Models;

public partial class Direction
{
    public int Id { get; set; }

    public string DirectionName { get; set; } = null!;

    public virtual ICollection<EventTypeDictionary> EventTypeDictionary { get; set; } = new List<EventTypeDictionary>();

    public virtual ICollection<User> Iduser { get; set; } = new List<User>();
}
