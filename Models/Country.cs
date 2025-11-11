using System;
using System.Collections.Generic;

namespace uchebnayaPractica.Models;

public partial class Country
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string NameCountry { get; set; } = null!;

    public string NameEng { get; set; } = null!;

    public virtual ICollection<User> Iduser { get; set; } = new List<User>();
}
