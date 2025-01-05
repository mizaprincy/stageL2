using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Poste_travail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int id_poste { get; set; }

   
    [MaxLength(100)] // Limite de longueur à 100 caractères
    public required string design { get; set; }

    [Required]
    [Column(TypeName = "decimal(60, 2)")] // Montants décimaux
    public decimal salaire_horaire { get; set; }

    [Required]
    public int heures_travaillees { get; set; }

    public required string statut { get; set; }

    public string? description { get; set; }

    public ICollection<Employe>? Employes { get; set; }
}
