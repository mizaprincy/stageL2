using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PosteTravailController : ControllerBase
    {
        private readonly Gestion_EntrepriseDbContext _context;

        public PosteTravailController(Gestion_EntrepriseDbContext context)
        {
            _context = context;
        }

        //GET: PosteTravail/actifs/count
        [HttpGet("actifs/count")]
        public async Task<IActionResult> GetActivePostesCount()
        {
            var count = await _context.Postes
                .Where(p => p.statut.Trim().ToLower() == "actif")
                .CountAsync();

            return Ok(count);
        }

        // GET: PosteTravail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Poste_travail>>> GetPostes()
        {
            var poste = await _context.Postes
                .Include(p => p.Employes)
                .Where(p => p.statut == "actif")
                .ToListAsync();

            if (poste == null)
            {
                return NotFound();
            }

            return poste;
        }

        // GET: api/PosteTravail/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Poste_travail>> GetPoste(int id)
        {
            var poste = await _context.Postes.FindAsync(id);

            if (poste == null)
            {
                return NotFound();
            }

            return poste;
        }

        // POST: PosteTravail
        [HttpPost]
        public async Task<ActionResult<Poste_travail>> CreatePoste(PosteDataDto posteData)
        {
            try
            {
                var poste = await _context.Postes
                    .FirstOrDefaultAsync(p => p.design.ToLower() == posteData.design.ToLower());

                if (poste != null)
                {
                    return BadRequest(new { message = "Ce poste de travail existe déjà." });
                }

                var newPoste = new Poste_travail
                {
                    design = posteData.design,
                    salaire_horaire = posteData.salaire_horaire,
                    heures_travaillees = posteData.heures_travaillees,
                    statut = "actif",
                    description = posteData.description,
                };

                _context.Postes.Add(newPoste);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPoste), new { id = newPoste.id_poste }, newPoste);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception capturée : {ex.Message}");
                Console.WriteLine($"StackTrace : {ex.StackTrace}");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        public class PosteDataDto
        {
            public required string design { get; set; }
            public required int salaire_horaire { get; set; }
            public required int heures_travaillees { get; set; }
            public string? description { get; set; }
        }

        // PUT: PosteTravail/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPoste(int id, Poste_travail poste)
        {
            if (id != poste.id_poste)
            {
                return BadRequest();
            }
            //verifier si il y a un changement
            var localposte = _context.Postes.FirstOrDefault(p => p.id_poste == id);

            if (localposte == null) 
            { 
                return BadRequest(new {message = "Poste non trouvé" });
            }

            if (localposte.design == poste.design &&
                localposte.salaire_horaire == poste.salaire_horaire &&
                localposte.heures_travaillees == poste.heures_travaillees &&
                localposte.description == poste.description)
            {
                return BadRequest(new { message = "Aucun changement n'a été fait" });
            }

            _context.Entry(poste).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PosteExists(id))
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

        // DELETE: PosteTravail/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePoste(string id)
        {
            var poste = await _context.Postes.FindAsync(id);
            if (poste == null)
            {
                return NotFound(new { message = "Poste de travail non trouvé" });
            }
            var employeActif = await _context.Employes
                .Where(e => e.date_depart != null)
                .ToListAsync();

            if (employeActif != null) {
                return BadRequest(new { message = "Ce poste contient encore un employe actif" });
            }
            // Marquer le poste comme inactif
            poste.statut = "inactif";

            // Sauvegarder les modifications
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PosteExists(int id)
        {
            return _context.Postes.Any(p => p.id_poste == id);
        }
    }
}
