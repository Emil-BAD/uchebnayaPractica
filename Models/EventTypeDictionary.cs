using System;
using System.Collections.Generic;

namespace uchebnayaPractica.Models;

public partial class EventTypeDictionary
{
    public int Idevent { get; set; }

    public int Idtype { get; set; }

    public int Iddictionary { get; set; }

    public virtual Direction IddictionaryNavigation { get; set; } = null!;

    public virtual Event IdeventNavigation { get; set; } = null!;

    public virtual TypeofEvent IdtypeNavigation { get; set; } = null!;
}
