using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TippPlattform.Models;
using TippPlattform.Services;

namespace TippPlattform.Controllers
{
    /// <summary>
    /// Partial View Controller for displaying game results and user tips in a specific group and game day.
    /// </summary>
    public class SpielergebnisController : Controller
    {
        private readonly TippPlattformContext _context;
        private readonly TippPlattformService _tippPlattformService;
        public SpielergebnisController(TippPlattformContext context, TippPlattformService tippPlattformService)
        {
            _context = context;
            _tippPlattformService = tippPlattformService;
        }
        // Partialview of game's results with user's tipp
        public IActionResult Index(int? gruppeId, int spielTag)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value!);
            var viewModel = new SpielergebnisViewModel
            {
                SpielMitUserTipps = _context.SpieleInTippgruppen
                // Get all games in the group on the specified game day
                .Where(s => s.TippgruppeId == gruppeId 
                    && s.Spiel.Spieltag == spielTag)
                .Include(s => s.Spiel)
                    .ThenInclude(sp => sp.TeamA)
                .Include(s => s.Spiel)
                    .ThenInclude(sp => sp.TeamB)
                .Include(s => s.Spiel)
                    .ThenInclude(sp => sp.Tippscheines)
                .OrderByDescending(s=>s.Spiel.SpielBeginn)
                .ThenBy(s=>s.SpielId)
                .Select(s => new SpielMitUserTipp
                {
                    Spiel = s.Spiel,
                    // Here get the user's tip for the game
                    UserTipp = s.Spiel.Tippscheines.FirstOrDefault(t => t.UserId == userId)
                })
                .ToList()
            };


            return PartialView("_Spielergebnis", viewModel);
        }
    }
}
