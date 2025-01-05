using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class JwtSettings
    {
        [Required(ErrorMessage = "La clé secrète est requise.")]
        [StringLength(100, ErrorMessage = "La clé secrète ne doit pas dépasser 100 caractères.")]
        public required string SecretKey { get; set; }

        [Required(ErrorMessage = "L'émetteur est requis.")]
        [StringLength(100, ErrorMessage = "L'émetteur ne doit pas dépasser 100 caractères.")]
        public required string Issuer { get; set; }

        [Required(ErrorMessage = "L'audience est requise.")]
        [StringLength(100, ErrorMessage = "L'audience ne doit pas dépasser 100 caractères.")]
        public required string Audience { get; set; }

        [Required(ErrorMessage = "La durée d'expiration en minutes est requise.")]
        [Range(1, 1440, ErrorMessage = "L'expiration en minutes doit être entre 1 et 1440 (24 heures).")]
        public required int ExpirationInMinutes { get; set; }
    }
}
