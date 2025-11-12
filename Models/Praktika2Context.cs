using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace uchebnayaPractica.Models;

public partial class Praktika2Context : DbContext
{
    public Praktika2Context()
    {
    }

    public Praktika2Context(DbContextOptions<Praktika2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activity { get; set; }

    public virtual DbSet<ActivityEvent> ActivityEvent { get; set; }

    public virtual DbSet<City> City { get; set; }

    public virtual DbSet<Country> Country { get; set; }

    public virtual DbSet<Direction> Direction { get; set; }

    public virtual DbSet<Event> Event { get; set; }

    public virtual DbSet<EventActivityJury> EventActivityJury { get; set; }

    public virtual DbSet<EventTypeDictionary> EventTypeDictionary { get; set; }

    public virtual DbSet<Gender> Gender { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<TypeofEvent> TypeofEvent { get; set; }

    public virtual DbSet<User> User { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=EMILBAD;Database=Praktika2;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<ActivityEvent>(entity =>
        {
            entity.HasKey(e => new { e.Idactivity, e.Idevent });

            entity.ToTable("Activity_Event");

            entity.Property(e => e.Idactivity).HasColumnName("IDActivity");
            entity.Property(e => e.Idevent).HasColumnName("IDEvent");
            entity.Property(e => e.IduserModer).HasColumnName("IDUser_Moder");

            entity.HasOne(d => d.IdactivityNavigation).WithMany(p => p.ActivityEvent)
                .HasForeignKey(d => d.Idactivity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activity_Event_Activity");

            entity.HasOne(d => d.IdeventNavigation).WithMany(p => p.ActivityEvent)
                .HasForeignKey(d => d.Idevent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activity_Event_Event");

            entity.HasOne(d => d.IduserModerNavigation).WithMany(p => p.ActivityEvent)
                .HasForeignKey(d => d.IduserModer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activity_Event_User");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<Direction>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Iduser).HasColumnName("IDUser");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.Event)
                .HasForeignKey(d => d.Iduser)
                .HasConstraintName("FK_Event_User");

            entity.HasMany(d => d.Idcity).WithMany(p => p.Idevent)
                .UsingEntity<Dictionary<string, object>>(
                    "CityEvent",
                    r => r.HasOne<City>().WithMany()
                        .HasForeignKey("Idcity")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_City_Event_City"),
                    l => l.HasOne<Event>().WithMany()
                        .HasForeignKey("Idevent")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_City_Event_Event"),
                    j =>
                    {
                        j.HasKey("Idevent", "Idcity");
                        j.ToTable("City_Event");
                        j.IndexerProperty<int>("Idevent").HasColumnName("IDEvent");
                        j.IndexerProperty<int>("Idcity").HasColumnName("IDCity");
                    });
        });

        modelBuilder.Entity<EventActivityJury>(entity =>
        {
            entity.ToTable("Event_Activity_Jury");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Idactivity).HasColumnName("IDActivity");
            entity.Property(e => e.Idevent).HasColumnName("IDEvent");
            entity.Property(e => e.IduserJury).HasColumnName("IDUser_Jury");

            entity.HasOne(d => d.IdactivityNavigation).WithMany(p => p.EventActivityJury)
                .HasForeignKey(d => d.Idactivity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Activity_Jury_Activity");

            entity.HasOne(d => d.IdeventNavigation).WithMany(p => p.EventActivityJury)
                .HasForeignKey(d => d.Idevent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Activity_Jury_Event");

            entity.HasOne(d => d.IduserJuryNavigation).WithMany(p => p.EventActivityJury)
                .HasForeignKey(d => d.IduserJury)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Activity_Jury_User");
        });

        modelBuilder.Entity<EventTypeDictionary>(entity =>
        {
            entity.HasKey(e => new { e.Idevent, e.Idtype, e.Iddictionary });

            entity.ToTable("Event_Type_Dictionary");

            entity.Property(e => e.Idevent).HasColumnName("IDEvent");
            entity.Property(e => e.Idtype).HasColumnName("IDType");
            entity.Property(e => e.Iddictionary).HasColumnName("IDDictionary");

            entity.HasOne(d => d.IddictionaryNavigation).WithMany(p => p.EventTypeDictionary)
                .HasForeignKey(d => d.Iddictionary)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Type_Dictionary_Direction");

            entity.HasOne(d => d.IdeventNavigation).WithMany(p => p.EventTypeDictionary)
                .HasForeignKey(d => d.Idevent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Type_Dictionary_Event");

            entity.HasOne(d => d.IdtypeNavigation).WithMany(p => p.EventTypeDictionary)
                .HasForeignKey(d => d.Idtype)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Type_Dictionary_TypeofEvent");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TypeofEvent>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Fio).HasColumnName("FIO");

            entity.HasMany(d => d.Idcountry).WithMany(p => p.Iduser)
                .UsingEntity<Dictionary<string, object>>(
                    "UserCountry",
                    r => r.HasOne<Country>().WithMany()
                        .HasForeignKey("Idcountry")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_User_Country_Country"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Iduser")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_User_Country_User"),
                    j =>
                    {
                        j.HasKey("Iduser", "Idcountry");
                        j.ToTable("User_Country");
                        j.IndexerProperty<int>("Iduser").HasColumnName("IDUser");
                        j.IndexerProperty<int>("Idcountry").HasColumnName("IDCountry");
                    });

            entity.HasMany(d => d.Iddirection).WithMany(p => p.Iduser)
                .UsingEntity<Dictionary<string, object>>(
                    "UserDirection",
                    r => r.HasOne<Direction>().WithMany()
                        .HasForeignKey("Iddirection")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_User_Direction_Direction"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Iduser")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_User_Direction_User"),
                    j =>
                    {
                        j.HasKey("Iduser", "Iddirection");
                        j.ToTable("User_Direction");
                        j.IndexerProperty<int>("Iduser").HasColumnName("IDUser");
                        j.IndexerProperty<int>("Iddirection").HasColumnName("IDDirection");
                    });

            entity.HasMany(d => d.Idevent).WithMany(p => p.Iduser1)
                .UsingEntity<Dictionary<string, object>>(
                    "UserEvent",
                    r => r.HasOne<Event>().WithMany()
                        .HasForeignKey("Idevent")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_User_Event_Event"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Iduser")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_User_Event_User"),
                    j =>
                    {
                        j.HasKey("Iduser", "Idevent");
                        j.ToTable("User_Event");
                        j.IndexerProperty<int>("Iduser").HasColumnName("IDUser");
                        j.IndexerProperty<int>("Idevent").HasColumnName("IDEvent");
                    });

            entity.HasMany(d => d.Idgender).WithMany(p => p.Iduser)
                .UsingEntity<Dictionary<string, object>>(
                    "UserGender",
                    r => r.HasOne<Gender>().WithMany()
                        .HasForeignKey("Idgender")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_User_Gender_Gender"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Iduser")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_User_Gender_User"),
                    j =>
                    {
                        j.HasKey("Iduser", "Idgender");
                        j.ToTable("User_Gender");
                        j.IndexerProperty<int>("Iduser").HasColumnName("IDUser");
                        j.IndexerProperty<int>("Idgender").HasColumnName("IDGender");
                    });

            entity.HasMany(d => d.Idrole).WithMany(p => p.Iduser)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("Idrole")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_User_Role_Role"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Iduser")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_User_Role_User"),
                    j =>
                    {
                        j.HasKey("Iduser", "Idrole");
                        j.ToTable("User_Role");
                        j.IndexerProperty<int>("Iduser").HasColumnName("IDUser");
                        j.IndexerProperty<int>("Idrole").HasColumnName("IDRole");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
