using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TippPlattform.Models;

namespace TippPlattform.Controllers
{
    /// <summary>
    /// LeaderboardController verwaltet die Anzeige der Punktestände in den Tippgruppen.
    /// </summary>
    public class LeaderboardController : Controller
    {
        private readonly TippPlattformContext _context;
        public LeaderboardController(TippPlattformContext context)
        {
            _context = context;
        }
        // Partialview showing score of users in the group
        public IActionResult Index(int gruppeId, int spielTag)
        {
            // Get all users that joins the group
            var leaderBoard = _context.Beitritte 
                .Where(x => x.TippgruppeId == gruppeId)
                .Include(x => x.User)
                .Select(x => new ScoreboardEntry
                {
                    Username = x.User.Username ?? "Username nicht gefunden!",
                    Points = x.Points,
                    LastTipp = _context.Tippscheine 
                        .Include(s => s.Spiel)
                        // Shows only tips for the current group, current user, and current game day 
                        .Where(t => t.TippgruppeId == gruppeId && t.UserId == x.User.Id  && t.Spiel.Spieltag == spielTag) 
                        .OrderByDescending(t => t.Spiel.SpielBeginn)
                        .ThenBy(t=>t.SpielId)
                        .Select(t => new LastTippEntry
                        {
                            SpielId = t.SpielId,
                            UserTipp = $"{t.TippA} : {t.TippB}",
                            UserGewinn = t.Points ?? 0
                        })
                        .ToList()
                })
                .OrderByDescending(x => x.Points)
                .ToList();

            // All games in the group
            var spiele = _context.SpieleInTippgruppen
                        .Include(s => s.Spiel)
                        .Include(s => s.Spiel.TeamA)
                        .Include(s => s.Spiel.TeamB)
                        .Where(t => t.TippgruppeId == gruppeId
                        && t.Spiel.TeamAScore != null
                        && t.Spiel.Spieltag == spielTag)
                        .OrderByDescending(t => t.Spiel.SpielBeginn)
                        .ThenBy(t => t.SpielId);

            var leaderboardViewModel = new LeaderboardViewModel
            {
                Leaderboard = leaderBoard,
                LastGames = spiele
                        .Select(s => new LastGamesEntry
                        {
                            SpielId = s.SpielId,
                            TeamA = s.Spiel.TeamA.Name ?? "",
                            TeamAScore = s.Spiel.TeamAScore ?? 0,
                            TeamB = s.Spiel.TeamB.Name ?? "",
                            TeamBScore = s.Spiel.TeamBScore ?? 0
                        })
                        .ToList()
            };
            return PartialView("_Leaderboard", leaderboardViewModel);
        }
    }
}
