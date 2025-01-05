using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    public class Gestion_EntrepriseDbContext : DbContext
    {
        public Gestion_EntrepriseDbContext(DbContextOptions<Gestion_EntrepriseDbContext> options) : base(options) { }

        public DbSet<Employe> Employes { get; set; } = null!;
        public DbSet<Poste_travail> Postes { get; set; } = null!;
        public DbSet<Salaire> Salaires { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<DemandeAvance> DemandeAvances { get; set; } = null!;
        public DbSet<Inbox> Inboxes { get; set; } = null!;
        public DbSet<Avance> Avances { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des entités
            modelBuilder.Entity<Employe>()
                .ToTable("Employe")
                .HasKey(e => e.id_employe);

            modelBuilder.Entity<Poste_travail>()
                .ToTable("Poste_travail")
                .HasKey(p => p.id_poste);

            modelBuilder.Entity<Salaire>()
                .ToTable("Salaire")
                .HasKey(s => new { s.id_salaire, s.mois, s.annee });

            modelBuilder.Entity<User>()
                .ToTable("User")
                .HasKey(u => u.UserId);

            modelBuilder.Entity<DemandeAvance>()
                .ToTable("DemandeAvance")
                .HasKey(d => new { d.EmployeId, d.DemandeId });

            modelBuilder.Entity<Avance>()
                .ToTable("Avance")
                .HasKey(a => a.AvanceId);

            modelBuilder.Entity<Inbox>()
                .ToTable("Inbox")
                .HasKey(i => i.InboxId);

            // Configuration des relations
            modelBuilder.Entity<Employe>()
                .HasOne(e => e.Poste_Travail)
                .WithMany(p => p.Employes)
                .HasForeignKey(e => e.id_poste)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Salaire>()
                .HasOne(s => s.Employe)
                .WithMany(e => e.Salaires)
                .HasForeignKey(s => s.id_employe)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Employe)
                .WithOne(e => e.User)
                .HasForeignKey<User>(u => u.EmployeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_User_Employe");

            modelBuilder.Entity<DemandeAvance>()
                .HasOne(d => d.Employe)
                .WithMany()
                .HasConstraintName("FK_DemandeAvance_Employe")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Avance>()
                .HasOne(a => a.Employe)
                .WithMany(e => e.Avances) // Un Employé peut avoir plusieurs avances
                .HasForeignKey(a => a.id_employe)
                .OnDelete(DeleteBehavior.Cascade) // Suppression en cascade si l'employé est supprimé
                .HasConstraintName("FK_Avance_Employe");

            // Configuration de la relation avec DemandeAvance et Employe
            modelBuilder.Entity<Inbox>()
                .HasOne(i => i.DemandeAvance)
                .WithMany() // Une DemandeAvance peut avoir plusieurs Inbox
                .HasForeignKey(i => new { i.EmployeId, i.DemandeId }) // La clé étrangère vers DemandeAvance
                .OnDelete(DeleteBehavior.Cascade); // Suppression en cascade

            // Configuration de la relation avec User (AdminId et EmployeId)
            modelBuilder.Entity<Inbox>()
                .HasOne(i => i.Admin)
                .WithMany()
                .HasForeignKey(i => i.AdminId) // La clé étrangère vers Admin
                .OnDelete(DeleteBehavior.SetNull); // Admin est optionnel

            modelBuilder.Entity<Inbox>()
                .HasOne(i => i.Employe)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull); // Employé est optionnel
        }

    }
}
