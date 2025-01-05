using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AvanceController : Controller
    {
        private readonly Gestion_EntrepriseDbContext _context;

        public AvanceController(Gestion_EntrepriseDbContext context)
        {
            _context = context;
        }

        //GET Avance/55F23/01/2024
        [HttpGet("avance/valeur")]
        public IActionResult GetAvanceDeduction(string idEmploye, int mois, int annee)
        {
            // Vérifier si l'employé existe
            var employe = _context.Employes.FirstOrDefault(e => e.id_employe == idEmploye);
            if (employe == null)
            {
                return NotFound(new { message = "Employé non trouvé." });
            }

            // Recherche de l'avance active pour l'employé
            var avance = _context.Avances
                .FirstOrDefault(a => a.id_employe == idEmploye && a.MontantRestant > 0);

            if (avance == null)
            {
                return Ok(new { message = "Aucune avance active pour cet employé.", deduction = 0.00 });
            }

            // Calculer la valeur de la tranche pour l'avance
            decimal montantParTranche = avance.MontantTotal / avance.TranchesTotales;

            // Vérification si une tranche doit être appliquée pour le mois et l'année spécifiés
            if (
                avance.DateProchaineTranche!.Value.Year == annee &&
                avance.DateProchaineTranche.Value.Month == mois)
            {
                // Retourne la valeur de la tranche à déduire
                return Ok(new
                {
                    message = "Tranche applicable pour le salaire.",
                    deduction = montantParTranche
                });
            }

            // Si aucune tranche n'est applicable pour ce mois
            return Ok(new 
            { 
                message = "Aucune tranche à déduire pour ce mois.",
                deduction = 0.00
            });
        }

    }
}
