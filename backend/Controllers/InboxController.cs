using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InboxController : ControllerBase
    {
        private readonly Gestion_EntrepriseDbContext _context;

        public InboxController(Gestion_EntrepriseDbContext context)
        {
            _context = context;
        }

        [HttpGet("admin")]
        public IActionResult GetInboxAdmin(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Role == "Admin");
            if (user == null)
            {
                return BadRequest("Admin non trouvé.");
            }

            var inboxMessages = _context.Inboxes
                .Include(i => i.DemandeAvance!)
                .ThenInclude(d => d.Employe)
                .Where(i => i.AdminId == user.UserId && i.DateNotification.Month == DateTime.Now.Month)
                .OrderByDescending(i => i.DateNotification)
                .ToList();

            return Ok(inboxMessages);
        }


        [HttpGet("employe")]
        public IActionResult GetInboxEmploye()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == User.Identity!.Name);
            if (user == null)
            {
                return BadRequest("Utilisateur non trouvé.");
            }

            var inboxMessages = _context.Inboxes
                .Where(i => i.EmployeId == user.Employe!.id_employe)
                .OrderByDescending(i => i.DateNotification)
                .ToList();

            return Ok(inboxMessages);
        }

        // Marquer une notification comme lue
        [HttpPut("inbox/markAsRead/{inboxId}")]
        public IActionResult MarkInboxAsRead(int inboxId)
        {
            var inboxMessage = _context.Inboxes.FirstOrDefault(i => i.InboxId == inboxId);
            if (inboxMessage == null)
            {
                return BadRequest("Message Inbox non trouvé.");
            }

            inboxMessage.IsRead = true;
            _context.SaveChanges();

            return Ok(inboxMessage);
        }

    }
}
