using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class SignupRequest
    {
        [StringLength(50)]
        public required string Nom { get; set; }

        [StringLength(50)]
        public required string Prenom { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public required string Email { get; set; }

        [StringLength(100, ErrorMessage = "Password non valide", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        public string? EmployeId { get; set; }
    }
}
