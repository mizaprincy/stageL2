using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

namespace backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeController : ControllerBase
    {
        private readonly Gestion_EntrepriseDbContext _context;

        public EmployeController(Gestion_EntrepriseDbContext context)
        {
            _context = context;
        }

        // GET: Employe
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employe>>> GetEmployes()
        {
            var employe = await _context.Employes
                .Include(e => e.Poste_Travail)
                .Where(e => e.date_depart == null)
                .ToListAsync();
            if (employe == null) { 
                return NotFound(new {message = "Aucun employe trouve"});
            }
            return employe;
        }

        // GET: Employe/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Employe>> GetEmploye(string id)
        {
            var employe = await _context.Employes.Include(e => e.Poste_Travail).FirstOrDefaultAsync(e => e.id_employe == id);

            if (employe == null)
            {
                return NotFound();
            }

            return employe;
        }

        //GET: Employe/actifs/count
        [HttpGet("actifs/count")]
        public async Task<IActionResult> GetActiveEmployesCount()
        {
            var count = await _context.Employes
                .Where(e => e.date_depart == null)
                .CountAsync();

            return Ok(count);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Employe>>> GetSpecificEmploye(int id_poste, string statut, DateOnly? embauche)
        {
            // Détermine la valeur booléenne attendue de date_depart pour le statut "actif" ou "inactif"
            bool isActive = statut?.ToLower() == "actif";

            var employes = await _context.Employes
                .Include(e => e.Poste_Travail)
                .Where(e =>
                    (id_poste == 0 || e.id_poste == id_poste) &&
                    (!embauche.HasValue || e.date_embauche == embauche) &&
                    ((isActive && e.date_depart == null) || (!isActive && e.date_depart != null))

                )
                .ToListAsync();

            if (employes == null)
            {
                Console.WriteLine($"Date embauche: {embauche}");
                return NotFound();
            }
            Console.WriteLine($"{id_poste} + {embauche} + {statut}");
            return Ok(employes);
        }


        // GET:  /Employe/search?name=John
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Employe>>> SearchEmployesByName(string name, int? id_poste, string? statut, DateOnly? embauche)
        {
            bool isActive = statut?.ToLower() == "actif";

            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Le nom ne peut pas être vide.");
            }

            var employes = await _context.Employes
                .Include(e => e.Poste_Travail)
                .Where(e => 
                        (e.nom.ToLower().Contains(name.ToLower()) || e.prenom.ToLower().Contains(name.ToLower())) &&
                        (id_poste == 0 || e.id_poste == id_poste) &&
                        (!embauche.HasValue || e.date_embauche == embauche) &&
                        ((isActive && e.date_depart == null) || (!isActive && e.date_depart != null))
                )
                .ToListAsync();

            return Ok(employes); // Retourne les employés trouvés, y compris une liste vide si aucun
        }


        // POST: Employe
        [HttpPost]
        public async Task<ActionResult<Employe>> CreateEmploye(Employe employeData)
        {

            //verifier si l'id est deja utilise
            var employe = await _context.Employes
                .FirstOrDefaultAsync(e => e.id_employe == employeData.id_employe);

            if (employe != null)
            {
                return BadRequest(new {message = "L'ID est deja utilisee par un employe"});
            }

            // Ajoute l'employé à la base de données
            _context.Employes.Add(employeData);
            await _context.SaveChangesAsync();

            // Retourne une réponse avec l'URL de l'employé nouvellement créé
            return CreatedAtAction(nameof(GetEmploye), new { id = employeData.id_employe }, employeData);
        }


        // PUT: Employe/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmploye(string id, Employe employe)
        {
            if (id != employe.id_employe)
            {
                return BadRequest(new {message = "ID differentes"});
            }
            //verifier si il y a un changement
            var employeData = await _context.Employes
                .FirstOrDefaultAsync(e => e.id_employe == employe.id_employe);

            if (employeData == null)
            {
                return NotFound(new {message = "Employé non trouvé" });
            }
            if (employeData.nom == employe.nom &&
                employeData.prenom == employe.prenom &&
                employeData.id_poste == employe.id_poste &&
                employeData.tel == employe.tel)
            {
                return BadRequest(new { message = "Aucun changement n'a été fait" });
            }

            _context.Entry(employe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: Employe/{id}/{dateDepart}
        [HttpDelete("{id}/{dateDepart}")]
        public async Task<IActionResult> DeleteEmploye(string id, string dateDepart)
        {
            // Conversion de la chaîne de date en DateOnly
            if (!DateOnly.TryParse(dateDepart, out DateOnly parsedDate))
            {
                return BadRequest(new {message = "Le format de la date est incorrect." });
            }

            var employe = await _context.Employes.FindAsync(id);
            if (employe == null)
            {
                return NotFound(new {message = "Employé non trouvé" });
            }

            // Marquer l'employé comme inactif en mettant à jour la date_depart
            employe.date_depart = parsedDate;

            // Sauvegarder les modifications
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool EmployeExists(string id)
        {
            return _context.Employes.Any(e => e.id_employe == id);
        }
    }
}
