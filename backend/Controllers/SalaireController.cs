using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SalaireController : ControllerBase
    {
        private readonly Gestion_EntrepriseDbContext _context;

        public SalaireController(Gestion_EntrepriseDbContext context)
        {
            _context = context;
        }

        // GET: Salaire
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Salaire>>> GetSalaires()
        {
            return await _context.Salaires.ToListAsync();
        }

        // GET: Salaire/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Salaire>> GetSalaire(int id)
        {
            var salaire = await _context.Salaires.FindAsync(id);

            if (salaire == null)
            {
                return NotFound();
            }

            return salaire;
        }

        // PUT: Salaire/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalaire(int id, Salaire salaire)
        {
            if (id != salaire.id_salaire)
            {
                return BadRequest();
            }

            _context.Entry(salaire).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalaireExists(id))
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

        // POST: Salaire
        [HttpPost]
        public async Task<ActionResult<Salaire>> PostSalaire([FromBody] SalaireDto salaireDto)
        {
            // Vérifier si l'employé existe
            var employe = await _context.Employes.Include(e => e.Poste_Travail).FirstOrDefaultAsync(e => e.id_employe == salaireDto.id_employe);
            if (employe == null)
            {
                return BadRequest(new { message = "Employé introuvable." });
            }

            // Vérifier si un salaire existe déjà pour ce mois et cette année
            var salaireExistant = await _context.Salaires
                .FirstOrDefaultAsync(s => s.id_employe == salaireDto.id_employe && s.mois == salaireDto.mois && s.annee == salaireDto.annee);

            if (salaireExistant != null)
            {
                return Conflict(new { message = "Un salaire pour cet employé, ce mois et cette année existe déjà." });
            }

            // Vérifier la présence du poste de travail
            if (employe.Poste_Travail == null)
            {
                return BadRequest(new { message = "Le poste de travail de l'employé est introuvable." });
            }

            // Calculer le montant brut (salaire_horaire * heures_travaillees)
            var montantBrut = employe.Poste_Travail.salaire_horaire * employe.Poste_Travail.heures_travaillees;

            // Récupérer l'avance pour ce mois et cette année
            var avance = await _context.Avances
                .FirstOrDefaultAsync(a => a.id_employe == salaireDto.id_employe && a.DateProchaineTranche!.Value.Month == salaireDto.mois && a.DateProchaineTranche.Value.Year == salaireDto.annee);

            decimal montantAvanceDeduit = 0m;

            if (avance != null || avance?.Statut == "En cours")
            {
                if (avance.Type == "quinzaine")
                {
                    // Déduire entièrement l'avance quinzaine
                    montantAvanceDeduit = avance.MontantRestant;
                    avance.MontantRestant = 0; // Avance remboursée en totalité
                    avance.Statut = "Terminée";
                    avance.DateProchaineTranche = null;
                }
                else if (avance.Type == "exceptionnelle")
                {
                    if (avance.TranchesRestantes <= 0 || avance.MontantRestant <= 0)
                    {
                        return BadRequest(new { message = "L'avance est déjà terminée." });
                    }

                    // Calculer la tranche pour ce salaire
                    decimal montantTranche = avance.MontantRestant / avance.TranchesRestantes;

                    // A la derniere tranche, on enleve tout le reste
                    if (avance.TranchesRestantes == 1)
                    {
                        montantTranche = avance.MontantRestant;

                        if(montantTranche > montantBrut)
                        {
                            montantTranche = montantBrut;
                        }
                    }

                    // Appliquer le montant de l'avance au salaire brut
                    montantAvanceDeduit = montantTranche;

                    // Mettre à jour l'avance
                    avance.MontantRestant -= montantTranche;
                    avance.TranchesRestantes--;
                    

                    if (avance.TranchesRestantes > 0 || avance.MontantRestant > 0)
                    {
                        avance.DateProchaineTranche = avance.DateProchaineTranche!.Value.AddMonths(1); //Paiement mensuel

                    }
                    else
                    {
                        avance.Statut = "Terminée"; // Marquer l'avance comme remboursée
                        avance.DateProchaineTranche = null; // Plus de prochaine tranche
                    }
                }

                // Mettre à jour l'avance
                _context.Avances.Update(avance);
            }

            // Calculer le salaire net après déduction
            var montantNet = montantBrut - montantAvanceDeduit;

            if (montantNet < 0)
            {
                return BadRequest(new { message = "Le montant net du salaire ne peut pas être négatif après déduction de l'avance." });
            }

            // Créer l'entité Salaire
            var salaire = new Salaire
            {
                id_employe = salaireDto.id_employe,
                montant = montantNet,
                date_paiement = salaireDto.date_paiement,
                mois = salaireDto.mois,
                annee = salaireDto.annee,
                avance = montantAvanceDeduit
            };

            _context.Salaires.Add(salaire);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSalaire), new { id = salaire.id_salaire }, salaire);
        }

        public class SalaireDto
        {
            public required string id_employe { get; set; }
            public required DateOnly date_paiement { get; set; }
            public required int mois { get; set; }
            public required int  annee { get; set; }
        }


        // DELETE: Salaire/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalaire(int id)
        {
            var salaire = await _context.Salaires.FindAsync(id);
            if (salaire == null)
            {
                return NotFound();
            }

            _context.Salaires.Remove(salaire);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SalaireExists(int id)
        {
            return _context.Salaires.Any(e => e.id_salaire == id);
        }

        [HttpGet("paiement/details")]
        public async Task<IActionResult> GetDetailsPaiement(string id_employe, DateOnly datePaiement, int mois, int annee)
        {
            // Vérifier si l'employé existe
            var employe = await _context.Employes
                .Include(e => e.Poste_Travail)
                .FirstOrDefaultAsync(e => e.id_employe == id_employe);

            if (employe == null)
            {
                return NotFound(new { message = "Employé introuvable." });
            }

            // Vérifier si une avance existe pour ce mois et cette année
            var avance = await _context.Avances
                .FirstOrDefaultAsync(a =>
                    a.id_employe == id_employe &&
                    a.DateProchaineTranche.HasValue &&
                    a.DateProchaineTranche.Value.Month == mois &&
                    a.DateProchaineTranche.Value.Year == annee);

            // Initialiser les données à renvoyer
            var montantTotal = employe.Poste_Travail!.salaire_horaire * employe.Poste_Travail.heures_travaillees;

            // Préparer les informations concernant l'avance
            decimal montantAvance = 0m;
            string typeAvance = "Aucune";
            int trancheActuelle = 0;
            int nombreTranches = 0;

            if (avance != null)
            {
                montantAvance = avance.MontantRestant / avance.TranchesRestantes; // Montant à déduire pour cette tranche
                typeAvance = avance.Type; // Quinzaine ou Exceptionnelle
                trancheActuelle = avance.TranchesTotales - avance.TranchesRestantes + 1; // Tranche actuelle (1ère, 2ème, ...)
                nombreTranches = avance.TranchesTotales;
            }

            // Préparer les données finales
            var detailsPaiement = new
            {
                EmployeId = employe.id_employe,
                EmployeNom = employe.nom,
                EmployePrenom = employe.prenom,
                EmployePoste = employe.Poste_Travail.design,
                MontantTotal = montantTotal,
                Avance = new
                {
                    MontantAvance = montantAvance,
                    TypeAvance = typeAvance,
                    TrancheActuelle = trancheActuelle,
                    NombreTranches = nombreTranches
                },
                DatePaiement = datePaiement,
                Mois = mois,
                Annee = annee
            };

            return Ok(detailsPaiement);
        }

        [HttpGet("salaires/par-annee")]
        public async Task<IActionResult> GetSalairesParAnnee(string id_employe, int annee)
        {
            // Vérifier si l'employé existe
            var employe = await _context.Employes
                .FirstOrDefaultAsync(e => e.id_employe == id_employe);

            if (employe == null)
            {
                return NotFound(new { message = "Employé introuvable." });
            }

            // Récupérer les salaires de l'année donnée
            var salaires = await _context.Salaires
                .Where(s => s.id_employe == id_employe && s.annee == annee)
                .OrderBy(s => s.mois) // Trier par mois pour un affichage ordonné
                .Select(s => new
                {
                    Mois = s.mois,
                    Montant = s.montant,
                    DatePaiement = s.date_paiement,
                    AvanceDeduite = s.avance
                })
                .ToListAsync();

            // Vérifier si aucun salaire n'a été trouvé
            if (salaires.Count == 0)
            {
                return NotFound(new { message = "Aucun salaire trouvé pour cette année." });
            }

            // Préparer la réponse
            var result = new
            {
                EmployeId = employe.id_employe,
                EmployeNom = employe.nom,
                EmployePrenom = employe.prenom,
                Annee = annee,
                Salaires = salaires
            };

            return Ok(result);
        }


    }
}
