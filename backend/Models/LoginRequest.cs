using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class LoginRequest
    {
        [EmailAddress]
        [StringLength(100)]
        public required string Email { get; set; }

        [StringLength(100, ErrorMessage = "Password non valide", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
