using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DemandeAvance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DemandeId { get; set; }

    [Key,StringLength(100)]
    public required string EmployeId { get; set; }

    public int EmployeUserId { get; set; }

    public User? Employe { get; set; } 

    [Required]
    [Column(TypeName = "decimal(60, 2)")]
    public decimal Montant { get; set; }

    [Required]
    public int NombreTranches { get; set; }

    [Required]
    public DateOnly DateDemande { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [StringLength(50)]
    public required string Statut { get; set; }

    [StringLength(50)]
    public required string TypeAvance { get; set; }

    public string? Message { get; set; } // Message pour notifier l'employé
}
