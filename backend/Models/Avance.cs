using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Avance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AvanceId { get; set; }

    [Required, StringLength(100)]
    public required string id_employe { get; set; }

    [ForeignKey(nameof(id_employe))]
    public Employe? Employe { get; set; }

    [Required]
    [Column(TypeName = "decimal(60, 2)")]
    public decimal MontantTotal { get; set; } 

    [Required]
    [Column(TypeName = "decimal(60, 2)")]
    public decimal MontantRestant { get; set; } 

    [Required]
    public int TranchesTotales { get; set; }

    [Required]
    public int TranchesRestantes { get; set; }

    [Required]
    [StringLength(50)]
    public required string Type { get; set; }

    [Required]
    [StringLength(50)]
    public string Statut { get; set; } = "En cours"; // "En cours", "Terminé"

    public DateOnly DateDebut { get; set; } // Date de début du remboursement

    public DateOnly? DateProchaineTranche { get; set; }
}
