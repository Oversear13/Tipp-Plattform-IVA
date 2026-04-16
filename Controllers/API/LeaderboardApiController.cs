using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TippPlattform.Models;
using TippPlattformMaui.Shared.Dtos;

namespace TippPlattform.Controllers.API;
/// <summary>
/// Schnittstelle für App und Datenbank durch eine Api-Anbindung
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LeaderboardApiController : ControllerBase
{
    private readonly TippPlattformContext _context;
    public LeaderboardApiController(TippPlattformContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Liefert das Leaderboard für eine bestimmte Tippgruppe
    /// </summary>
    /// <param name="gruppeId">Die ID der Tippgruppe, für die das Leaderboard abgefragt wird</param>
    /// <returns>Eine Liste von <see cref="LeaderboardDto"/>-Objekten, sortiert nach Punkten in absteigender Reihenfolge.</returns>
    [HttpGet("GetLeaderboard")]
    [AllowAnonymous]
    public ActionResult<List<LeaderboardDto>> GetLeaderboard(int gruppeId)
    {
        var leaderboardList = _context.Beitritte
            .Where(x => x.TippgruppeId == gruppeId)
            .Include(x => x.User)
            .OrderByDescending(x => x.Points)
            .ToList();

        var leaderboard = leaderboardList
            .Select((x, index) => new LeaderboardDto
            {
                Platz = index + 1,
                Name = x.User.Username ?? "",
                Punkte = x.Points
            })
            .ToList();

        return leaderboard;
    }

}
