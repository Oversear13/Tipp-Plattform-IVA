using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TippPlattform.Models;

namespace TippPlattform.Controllers
{
    /// <summary>
    /// Controller for handling user Inviations.
    /// </summary>
    public class NachrichtenController : Controller
    {
        private readonly TippPlattformContext _context;
        public NachrichtenController(TippPlattformContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Displays the list of messages for the current user.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var userId = User.FindFirst("UserId")?.Value;

            var Nachrichten = _context.Nachrichten.Where(x=>x.EmpfaengerId.ToString() == userId)
                .Include(x=>x.Sender)
                .OrderByDescending(x => x.SendDatum)
                .ToList();
            if(Nachrichten.Any())
            {
                foreach (var nachricht in Nachrichten)
                {
                    if (nachricht.GelesenDatum == null)
                    {
                        nachricht.GelesenDatum = DateTime.UtcNow;
                        _context.SaveChanges();
                    }
                }
            }
            return View(Nachrichten);
        }
        /// <summary>
        /// Accepts a group invitation and adds the user to the group.
        /// </summary>
        /// <param name="id">Message ID</param>
        /// <returns> redirect to that group</returns>
        public IActionResult Accept(int id)
        {
            var nachricht = _context.Nachrichten.FirstOrDefault(x => x.Id == id);
            if (nachricht == null)
            {
                return NotFound();
            }
            // Check if the user is already a member of the group
            var beitritt = _context.Beitritte
                .FirstOrDefault(b => b.TippgruppeId == nachricht.EingeladeneGruppeId && b.UserId == nachricht.EmpfaengerId);
            if(beitritt == null)
            {
                _context.Beitritte.Add(new Beitritt
                {
                    TippgruppeId = nachricht.EingeladeneGruppeId,
                    UserId = nachricht.EmpfaengerId,
                    JoinedAt = DateTime.Now,
                    Points = 0
                });
            }
            _context.SaveChanges();

            _context.Nachrichten.Remove(nachricht);

            return RedirectToAction("Tippen", "TippGruppen", new { gruppeId = nachricht.EingeladeneGruppeId});
        }
        /// <summary>
        /// Deletes a message by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var nachricht = _context.Nachrichten.FirstOrDefault(x => x.Id == id);
            if (nachricht == null)
            {
                return NotFound();
            }
            _context.Nachrichten.Remove(nachricht);
            _context.SaveChanges();
            return Ok();
        }
    }
}
