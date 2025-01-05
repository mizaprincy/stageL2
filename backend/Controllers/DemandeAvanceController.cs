using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DemandeAvanceController : ControllerBase
    {
        private readonly Gestion_EntrepriseDbContext _context;

        public DemandeAvanceController(Gestion_EntrepriseDbContext context)
        {
            _context = context;
        }

        // Récupérer toutes les demandes d'avance pour l'employé connecté
        [HttpGet("employe/demandes")]
        public IActionResult GetDemandeAvanceByEmploye()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == User.Identity!.Name);
            if (user == null)
            {
                return BadRequest(new {message = "Utilisateur non trouvé." });
            }

            var demandes = _context.DemandeAvances
                .Where(d => d.EmployeId == user.Employe!.id_employe)
                .OrderByDescending(d => d.DateDemande)
                .ToList();

            return Ok(demandes);
        }

        // Créer une nouvelle demande d'avance
        [HttpPost("employe/demander")]
        public IActionResult DemanderAvance([FromBody] DemandeAvanceRequest demandeAvanceRequest, string email)
        {
            //var user = _context.Users.FirstOrDefault(u => u.Email == User.Identity!.Name);
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return BadRequest(new {message = "Utilisateur non trouvé." });
            }

            var employe = _context.Employes
                .Include(e => e.Poste_Travail)
                .FirstOrDefault(e => e.id_employe == demandeAvanceRequest.EmployeId);

            if (employe == null)
            {
                return BadRequest(new {message = "Employé non trouvé." });
            }

            var avanceEnCours = _context.Avances
                .Where(a => a.id_employe == demandeAvanceRequest.EmployeId && a.MontantRestant > 0)
                .FirstOrDefault();

            if (avanceEnCours != null)
            {
                return BadRequest(new
                {
                    message = "L'employé a déjà une avance en cours. Demande refusée.",
                    avanceId = avanceEnCours.AvanceId,
                    montantRestant = avanceEnCours.MontantRestant
                });
            }

            //Date de demande
            DateOnly datedemande = DateOnly.FromDateTime(DateTime.Now);

            //Verifier si une demande d'avance est en attente
            var demandeEnCours = _context.DemandeAvances
                .Where(d => d.EmployeId == demandeAvanceRequest.EmployeId && d.DateDemande.Month == datedemande.Month && d.DateDemande.Year == datedemande.Year)
                .FirstOrDefault();

            if(demandeEnCours != null)
            {
                return BadRequest(new {message = "Une demande a deja ete faite par cet employe, pour ce mois, cette annee" });
            }

            decimal salaire = employe.Poste_Travail!.salaire_horaire * employe.Poste_Travail.heures_travaillees;

            // Vérification du type d'avance et des montants
            if (demandeAvanceRequest.TypeAvance == "quinzaine")
            {
                if (DateTime.Now.Day < 15)
                {
                    return BadRequest(new {message = "Les avances quinzaine ne peuvent être demandées qu'après le 15 du mois." });
                }

                if (demandeAvanceRequest.Montant > (salaire / 2))
                {
                    return BadRequest(new {message = $"Le montant maximal pour une avance quinzaine est de {salaire / 2} Ar." });
                }
                demandeAvanceRequest.NombreTranches = 1;
            }
            else if (demandeAvanceRequest.TypeAvance == "exceptionnelle")
            {
                if (demandeAvanceRequest.Montant > (salaire * 3))
                {
                    return BadRequest(new {message = $"Le montant maximal pour une avance exceptionnelle est de {salaire * 3} Ar." });
                }
            }
            else
            {
                return BadRequest(new {message = "Type d'avance invalide. Les types valides sont 'quinzaine' et 'exceptionnelle'." });
            }


            var demandeAvance = new DemandeAvance
            {
                EmployeId = demandeAvanceRequest.EmployeId,
                EmployeUserId = user.UserId,
                Employe = user,
                Montant = demandeAvanceRequest.Montant,
                NombreTranches = demandeAvanceRequest.NombreTranches,
                DateDemande = datedemande,
                Statut = "En attente", // "En attente", "Approuvée", "Déclinée"
                TypeAvance = demandeAvanceRequest.TypeAvance,
                Message = demandeAvanceRequest.Message ??  null
            };

            _context.DemandeAvances.Add(demandeAvance);
            _context.SaveChanges();

            // Créer une notification Inbox pour l'Admin
            var admin = _context.Users.FirstOrDefault(u => u.Role == "Admin");
            if (admin == null)
            {
                return (BadRequest(new {message = "Admin non trouve" })); 
            }    
            
            var inboxMessage = new Inbox
            {
                AdminId = admin.UserId,
                DemandeId = demandeAvance.DemandeId,
                DemandeAvance = demandeAvance,
                Message = $"Demande d'avance {demandeAvance.TypeAvance}: {demandeAvance.Montant} Ar.",
                DateNotification = DateOnly.FromDateTime(DateTime.Now),
                IsRead = false
            };

            _context.Inboxes.Add(inboxMessage);
            _context.SaveChanges();
            

            return Ok(demandeAvance);
        }

        // Nouvelle classe pour simplifier la requête
        public class DemandeAvanceRequest
        {
            public required string EmployeId { get; set; }
            public required decimal Montant { get; set; }
            public required int NombreTranches { get; set; }
            public required string TypeAvance { get; set; }
            public string? Message { get; set; }
        }

        //Recuperer le montant d'une avance specifique
        [HttpGet("admin/avance/{idEmploye}/{mois}/{annee}")]
        public IActionResult GetMontantAvanceApprouvee(string idEmploye, int mois, int annee)
        {
            // Récupérer le montant total des demandes d'avance approuvées pour cet employé, mois et année
            var montantAvance = _context.DemandeAvances
                .Where(d => d.EmployeId == idEmploye &&
                            d.Statut == "Approuvée" &&
                            d.DateDemande.Month == mois && 
                            d.DateDemande.Year == annee)
                .FirstOrDefaultAsync();

            // Si aucune avance n'a été trouvée, retourner 0
            if (montantAvance == null)
            {
                return Ok(0);
            }

            // Retourner le montant de l'avance approuvée
            return Ok(new { MontantAvance = montantAvance });
        }


        // Répondre à une demande d'avance (approbation ou rejet)
        [HttpPatch("admin/repondre")]
        public IActionResult RepondreDemande([FromBody] ReponseDemandeDto demandeDto)
        {
            // Vérification de l'utilisateur admin
            var user = _context.Users.FirstOrDefault(u => u.Email == demandeDto.Email);
            if (user == null)
            {
                return BadRequest(new {message = "Admin non trouvé." });
            }

            // Recherche de la demande concernée
            var demande = _context.DemandeAvances.FirstOrDefault(d => d.DemandeId == demandeDto.DemandeId);
            if (demande == null)
            {
                return BadRequest(new {message = "Demande d'avance non trouvée." });
            }

            // Vérification si la demande est déjà traitée
            if (demande.Statut == "Approuvée" || demande.Statut == "Declinée")
            {
                return BadRequest(new { message = "Cette demande a déjà été traitée." });
            }

            // Mise à jour du statut
            demande.Statut = demandeDto.Statut;

            // Si le statut est approuvé, créer une avance
            if (demandeDto.Statut == "Approuvée")
            {
                // Vérification si l'employé a déjà une avance active
                var avanceExistante = _context.Avances.FirstOrDefault(a => a.id_employe == demande.EmployeId && a.MontantRestant > 0);
                if (avanceExistante != null)
                {
                    return BadRequest(new { message = "L'employé a déjà une avance en cours." });
                }

                // Création de l'avance
                var nouvelleAvance = new Avance
                {
                    id_employe = demande.EmployeId,
                    MontantTotal = demande.Montant,
                    MontantRestant = demande.Montant,
                    TranchesTotales = demande.NombreTranches,
                    TranchesRestantes = demande.NombreTranches,
                    Type = demande.TypeAvance,
                    DateDebut = DateOnly.FromDateTime(DateTime.Now),
                    DateProchaineTranche = DateOnly.FromDateTime(DateTime.Now) //Paiement mensuel
                };
        
                _context.Avances.Add(nouvelleAvance);
            }

            _context.SaveChanges();



            // Ajout d'une notification dans l'Inbox
            var inboxMessage = new Inbox
            {
                EmployeId = demande.EmployeId,
                DemandeId = demande.DemandeId,
                DemandeAvance = demande,
                Message = $"Votre demande d'avance a été {demandeDto.Statut}.",
                DateNotification = DateOnly.FromDateTime(DateTime.Now),
                IsRead = false
            };

            _context.Inboxes.Add(inboxMessage);
            _context.SaveChanges();

            return Ok(new { message = "Statut mis à jour avec succès.", demande });
        }

        public class ReponseDemandeDto
        {
            public int DemandeId { get; set; }
            public required string Statut { get; set; }
            public required string Email { get; set; }
        }
        
    }
}
