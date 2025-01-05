using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Salaire
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int id_salaire { get; set; }

    public required string id_employe { get; set; }


    [Column(TypeName = "decimal(60, 2)")]
    public required decimal montant { get; set; }

    [Required]
    public required DateOnly date_paiement { get; set; }

    [Required]
    public int mois { get; set; }

    [Required]
    public int annee { get; set; }

    [Column(TypeName = "decimal(60, 2)")]
    public decimal? avance { get; set; }

    public Employe? Employe { get; set; }
}
