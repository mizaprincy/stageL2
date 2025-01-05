using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using backend.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Services
{
    public class JwtTokenService
    {
        private readonly JwtSettings _jwtSettings;

        // Constructeur qui récupère les paramètres de configuration de JwtSettings
        public JwtTokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        // Méthode pour générer un JWT
        public string GenerateToken(string username, string role)
        {
            if (string.IsNullOrEmpty(_jwtSettings.SecretKey))
            {
                throw new InvalidOperationException("The JWT secret key is missing in the configuration.");
            }
            Console.WriteLine("SecretKey: " + _jwtSettings.SecretKey); // Ajouter pour vérifier la valeur


            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
