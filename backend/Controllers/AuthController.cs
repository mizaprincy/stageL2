using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using backend.Models;
using System.Linq;
using backend.Services;
using Microsoft.AspNetCore.Identity.Data;
using LoginRequest = backend.Models.LoginRequest;
using IdentityLoginRequest = Microsoft.AspNetCore.Identity.Data.LoginRequest;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Gestion_EntrepriseDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(Gestion_EntrepriseDbContext context, JwtTokenService jwtTokenService)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _jwtTokenService = jwtTokenService;
        }

        // Méthode d'inscription (Signup)
        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignupRequest signupRequest)
        {
            if (!ModelState.IsValid)
            {
                // Récupérer tous les messages d'erreur de validation
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                // Retourner une réponse JSON contenant les erreurs
                return BadRequest(new { errors });
            }

            // Vérifier si l'EmployeId existe dans la base de données
            var employe = _context.Employes.FirstOrDefault(e => e.id_employe == signupRequest.EmployeId);
            if (string.IsNullOrEmpty(signupRequest.EmployeId) && employe == null)
            {
                return BadRequest(new {message = "L'ID d'employé spécifié n'existe pas." });
            }
            if (employe!.email != signupRequest.Email)
            {
                return BadRequest(new {message = "Email non conforme" });
            }
            // Vérifier si l'EmployeId est déjà associé à un autre utilisateur
            var existingUser = _context.Users.FirstOrDefault(u => u.EmployeId == signupRequest.EmployeId);
            if (existingUser != null)
            {
                return BadRequest(new {message = "Cet employé est déjà associé à un autre compte utilisateur." });
            }

            // Créer un nouvel utilisateur
            var user = new User
            {
                Nom = signupRequest.Nom,
                Prenom = signupRequest.Prenom,
                Email = signupRequest.Email,
                Password = signupRequest.Password,
                Role = "Employe", //Employe par defaut
                EmployeId = signupRequest.EmployeId
            };
            // Hachage du mot de passe
            user.Password = _passwordHasher.HashPassword(user, signupRequest.Password);
            // Ajouter l'utilisateur à la base de données
            _context.Users.Add(user);
            _context.SaveChanges();


            // Mettre à jour l'UserId de l'employé avec le UserId nouvellement créé
            employe!.UserId = user.UserId;
            _context.Entry(employe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Employes.Any(e => e.id_employe == signupRequest.EmployeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var userInfo = new
            {
                user.Nom,
                user.Prenom,
                user.Email,
                user.Role
            };

            return Ok(new { user = userInfo });
        }


        // Méthode de connexion (Login)
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                // Récupérer tous les messages d'erreur de validation
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                // Retourner une réponse JSON contenant les erreurs
                return BadRequest(new { errors });
            }

            // Validation des champs requis
            if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new {message = "Email et Password sont requis." });
            }

            // Trouver l'utilisateur dans la base de données
            var user = _context.Users.FirstOrDefault(u => u.Email == loginRequest.Email);

            if (user == null)
            {
                return Unauthorized(new {message = "Identifiants invalides" });
            }

            // Vérifier le mot de passe en utilisant PasswordHasher
            // Compare le mot de passe saisi (loginRequest.Password) avec le mot de passe haché stocké dans user.Password
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                Console.WriteLine("lol");
                return Unauthorized(new {message = "Mot de passe incorrect" });
            }

            dynamic userInfo = new
            {
                user.Nom,
                user.Prenom,
                user.Email,
                user.Role
            };

            if (user.Role == "Employe")
            {
                userInfo = new
                {
                    user.Nom,
                    user.Prenom,
                    user.Email,
                    user.Role,
                    user.EmployeId
                };
            }

            Console.WriteLine(userInfo);
            return Ok(new { user = userInfo });
        }


    }
}
