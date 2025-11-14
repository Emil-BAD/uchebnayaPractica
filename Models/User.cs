using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace uchebnayaPractica.Models;

public partial class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Fio { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Phone { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Image { get; set; } = null!;

    public virtual ICollection<ActivityEvent> ActivityEvent { get; set; } = new List<ActivityEvent>();

    public virtual ICollection<Event> Event { get; set; } = new List<Event>();

    public virtual ICollection<EventActivityJury> EventActivityJury { get; set; } = new List<EventActivityJury>();

    public virtual ICollection<Country> Idcountry { get; set; } = new List<Country>();

    public virtual ICollection<Direction> Iddirection { get; set; } = new List<Direction>();

    public virtual ICollection<Event> Idevent { get; set; } = new List<Event>();

    public virtual ICollection<Gender> Idgender { get; set; } = new List<Gender>();

    public virtual ICollection<Role> Idrole { get; set; } = new List<Role>();
}
