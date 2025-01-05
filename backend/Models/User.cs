using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }

    [StringLength(100)]
    public required string Nom { get; set; }

    [StringLength(100)]
    public required string Prenom { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; set; }

    [StringLength(100, ErrorMessage = "Password non valide", MinimumLength = 8)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [StringLength(100)]
    public required string Role { get; set; }

    [ForeignKey("Employe")]
    public string? EmployeId { get; set; }
    public Employe? Employe { get; set; }
}
