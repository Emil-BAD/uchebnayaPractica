using System;
using System.Collections.Generic;

namespace uchebnayaPractica.Models;

public partial class Gender
{
    public int Id { get; set; }

    public string GenderName { get; set; } = null!;

    public virtual ICollection<User> Iduser { get; set; } = new List<User>();
}
