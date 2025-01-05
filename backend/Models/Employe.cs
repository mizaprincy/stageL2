using backend.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

public class Employe
{
    [Key]
    [StringLength(100)]
    public required string id_employe { get; set; }
    public int? UserId { get; set; }

    [StringLength(100)]
    public required string nom { get; set; }

    [Required]
    [StringLength(100)]
    public required string prenom { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public required string email { get; set; }

    [Phone]
    [StringLength(100)]
    public required string tel { get; set; }

    public required DateOnly date_embauche { get; set; }

    public DateOnly? date_depart { get; set; }

    public required int id_poste { get; set; }

    public Poste_travail? Poste_Travail { get; set; }

    public User? User { get; set; }

    public ICollection<Salaire> Salaires { get; set; } = new List<Salaire>();

    public ICollection<Avance>? Avances { get; set; }
}
