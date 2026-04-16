using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TippPlattform.Models;

namespace TippPlattform.Controllers
{
    /// <summary>
    /// Partial View Controller for submitting tips in a specific Tippgruppe.
    /// </summary>
    public class TippabgabeController : Controller
    {
        private readonly TippPlattformContext _context;
        public TippabgabeController(TippPlattformContext context)
        {
            _context = context;
        }
        // NO USE
        public IActionResult Index()
        {
            return View();
        }
        // A page to submit tips for a specific Tippgruppe.
        public IActionResult Tippabgabe(int? gruppeId, int spielTag)
        {
            // Get User Id from Claims
            var userId = int.Parse(User.FindFirst("UserId")?.Value!);
            var TippGruppe = _context.Tippgruppen
                .Include(x => x.TippgruppeAdmins)
                .FirstOrDefault(x => x.Id == gruppeId); // Get the Tippgruppe by Id
            var tippScheineListe = _context.Tippscheine
                .Where(x => x.UserId == userId
                && x.TippgruppeId == gruppeId)
                .ToList();
            var viewModel = new TippabgabeViewModel()
            {
                TippGruppe = TippGruppe,
                SpieleInGruppe = _context.SpieleInTippgruppen
                    .Include(s => s.Spiel)
                    .ThenInclude(s => s.TeamA)
                    .Include(s => s.Spiel)
                    .ThenInclude(s => s.TeamB)
                    .Include(s=>s.PunkteRegel)
                    .Where(s => s.TippgruppeId == gruppeId // Only games in the Tippgruppe
                        && s.Spiel.SpielBeginn > DateTime.Now // Only games that haven't started yet
                        && s.Spiel.Spieltag == spielTag) // Only games of the specified Spieltag
                    .ToList(),
                Tippscheine = tippScheineListe,
                IstTippGruppeAdmin = TippGruppe != null ? TippGruppe.TippgruppeAdmins.Any(x => x.UserId == userId) : false
            };
            return PartialView("_Tippabgabe", viewModel); // Return the partial view with data
        }
        /// <summary>
        /// Submits a tip for a specific game in a Tippgruppe.
        /// </summary>
        /// <param name="gruppeId"></param>
        /// <param name="spielId"></param>
        /// <param name="tippA"></param>
        /// <param name="tippB"></param>
        public void TippAbgeben(int? gruppeId, int? spielId, int? tippA, int? tippB)
        {
            if (gruppeId == null || spielId == null || tippA == null || tippB == null) { return; }
            
            int userId = int.Parse(User.FindFirst("UserId")?.Value!);
            
            var getQuote = _context.SpieleInTippgruppen
                .Include(x => x.PunkteRegel)
                .FirstOrDefault(x => x.SpielId == spielId)?
                .PunkteRegel;

            int quote1 = getQuote?.Quote1 ?? 4;
            int quote2 = getQuote?.Quote2 ?? 3;
            int quote3 = getQuote?.Quote3 ?? 2;
            int quote4 = getQuote?.Quote4 ?? 0;

            // Check if the user has already submitted a tip for this game
            var existingTippschein = _context.Tippscheine.FirstOrDefault(
                x => x.UserId == userId &&
                x.TippgruppeId == gruppeId &&
                x.SpielId == spielId);

            if (existingTippschein != null)
            {
                // Update the existing tippschein
                existingTippschein.TippA = tippA.Value;
                existingTippschein.TippB = tippB.Value;
                existingTippschein.Quote1 = quote1;
                existingTippschein.Quote2 = quote2;
                existingTippschein.Quote3 = quote3;
                existingTippschein.Quote4 = quote4;

                _context.Update(existingTippschein);
            }
            else
            {
                var newTippSchein = new Tippschein()
                {
                    UserId = userId,
                    TippgruppeId = gruppeId.Value,
                    SpielId = spielId.Value,
                    TippA = tippA.Value,
                    TippB = tippB.Value,
                    Quote1 = quote1,
                    Quote2 = quote2,
                    Quote3 = quote3,
                    Quote4 = quote4,
                    CreatedAt = DateTime.Now
                };
                _context.Tippscheine.Add(newTippSchein);
            }

            _context.SaveChanges();
        }
    }
}