using System;
using System.Collections.Generic;

namespace uchebnayaPractica.Models;

public partial class Role
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Iduser { get; set; } = new List<User>();
}
