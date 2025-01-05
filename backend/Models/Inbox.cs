using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Inbox
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InboxId { get; set; }

    // Relation avec Admin (utilise AdminId comme clé étrangère)
    [ForeignKey("AdminId")]
    public int? AdminId { get; set; }
    public User? Admin { get; set; }

    // Relation avec Employe (clé étrangère vers User)
    public string? EmployeId { get; set; }
    public User? Employe { get; set; }

    // Relation avec DemandeAvance (clé étrangère vers DemandeAvance)
    public int DemandeId { get; set; }
    public DemandeAvance? DemandeAvance { get; set; }

    public required string Message { get; set; }

    public DateOnly DateNotification { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public bool IsRead { get; set; } = false;
}
