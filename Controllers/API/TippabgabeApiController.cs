using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TippPlattform.Models;
using TippPlattformMaui.Shared.Dtos;

namespace TippPlattform.Controllers.Api;
/// <summary>
/// API Controller für die Tippabgaben
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TippabgabeApiController : ControllerBase
{
    private readonly TippPlattformContext _context;
    public TippabgabeApiController(TippPlattformContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Liefert die Tippabgaben für eine bestimmte Gruppe und einen bestimmten User.
    /// </summary>
    /// <param name="gruppeId"></param>
    /// <param name="userId"></param>
    /// <returns>Liste von Tippabgabe-Daten in Json-Format</returns>
    [HttpGet("GetTippabgabe")]
    [AllowAnonymous]
    public ActionResult<List<TippabgabeDto>> GetTippabgabe(int gruppeId, int userId)
    {
        // Holt die Tippscheine für den angegebenen User und die Gruppe
        var tippScheineListe = _context.Tippscheine
                .Where(x => x.UserId == userId
                && x.TippgruppeId == gruppeId)
                .ToList();

        var tipps = _context.SpieleInTippgruppen
                        .Include(s => s.Spiel)
                        .ThenInclude(s => s.TeamA) // Inkludieren TeamA Name
                        .Include(s => s.Spiel)
                        .ThenInclude(s => s.TeamB) // Inkludieren TeamB Name
                        .Where(s => s.TippgruppeId == gruppeId // Nur Spiele in der angegebenen Tippgruppe
                            && s.Spiel.SpielBeginn > DateTime.Now) // Nur Spiele, die in der Zukunft liegen
                        .ToList()
                        .Select(s => new TippabgabeDto
                        {
                            SpielId = s.SpielId,
                            TeamA = s.Spiel.TeamA.Name ?? "",
                            TeamB = s.Spiel.TeamB.Name ?? "",
                            Datum = s.Spiel.SpielBeginn,
                            Ergebnis = s.Spiel.TeamAScore.HasValue && s.Spiel.TeamBScore.HasValue ? $"{s.Spiel.TeamAScore}:{s.Spiel.TeamBScore}" : null,
                            TippA = tippScheineListe.FirstOrDefault(ts => ts.SpielId == s.SpielId)?.TippA ?? null,
                            TippB = tippScheineListe.FirstOrDefault(ts => ts.SpielId == s.SpielId)?.TippB ?? null,
                            Gewinn = tippScheineListe.FirstOrDefault(ts => ts.SpielId == s.SpielId)?.Points ?? null,

                        }).ToList();
        return tipps;
    }
    /// <summary>
    /// API für speichert die Tipps für eine bestimmte Gruppe und einen bestimmten User.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="gruppeId"></param>
    /// <param name="tipps">Liste von Tipps</param>
    /// <returns></returns>
    /// 
    [HttpPost("SaveTipps")]
    [AllowAnonymous]
    public IActionResult SaveTipps([FromQuery] int userId, [FromQuery] int gruppeId, [FromBody] List<TippAbgebenDto> tipps)
    {
        foreach (var tipp in tipps)
        {
            if (tipp.TippA == null || tipp.TippB == null)
            {
                continue;
            }

            // Hole die Quote für das Spiel
            var getQuote = _context.SpieleInTippgruppen
                .Include(x => x.PunkteRegel)
                .FirstOrDefault(x => x.SpielId == tipp.SpielId)?
                .PunkteRegel;

            // Standardwerte (4,3,2,0) für die Quoten, falls keine Quote in der Datenbank gefunden wird
            int quote1 = getQuote?.Quote1 ?? 4; 
            int quote2 = getQuote?.Quote2 ?? 3;
            int quote3 = getQuote?.Quote3 ?? 2;
            int quote4 = getQuote?.Quote4 ?? 0;

            // Prüfe, ob ein Tippschein für den User und die Gruppe bereits existiert
            // Wenn ja, aktualisiere ihn, wenn nein, erstelle einen neuen Tippschein
            var existingTippschein = _context.Tippscheine.FirstOrDefault(
                x => x.UserId == userId &&
                x.TippgruppeId == gruppeId &&
                x.SpielId == tipp.SpielId);

            if (existingTippschein != null)
            {
                existingTippschein.TippA = tipp.TippA.Value;
                existingTippschein.TippB = tipp.TippB.Value;
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
                    TippgruppeId = gruppeId,
                    SpielId = tipp.SpielId,
                    TippA = tipp.TippA.Value,
                    TippB = tipp.TippB.Value,
                    Quote1 = quote1,
                    Quote2 = quote2,
                    Quote3 = quote3,
                    Quote4 = quote4,
                    CreatedAt = DateTime.Now
                };
                _context.Tippscheine.Add(newTippSchein);
            }
        }

        try
        {
            _context.SaveChanges();
            Console.WriteLine("Änderungen gespeichert."); //TODO Erfolgsmeldung anzeigen
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler beim Speichern: " + ex.Message); //TODO Fehlerbehandlung
            return BadRequest();
        }
    }

    [HttpGet("GetSpiele")]
    [AllowAnonymous]
    public ActionResult<List<SpielDto>> GetSpiele(int gruppeId, int userId)
    {
        var spiele = _context.SpieleInTippgruppen
            .Include(s => s.Spiel)
                .ThenInclude(s => s.TeamA)
            .Include(s => s.Spiel)
                .ThenInclude(s => s.TeamB)
            .Where(s => s.TippgruppeId == gruppeId)
            .Select(s => new SpielDto
            {
                Id = s.Spiel.Id,
                HeimMannschaft = s.Spiel.TeamA.Name,
                GastMannschaft = s.Spiel.TeamB.Name,
                Datum = s.Spiel.SpielBeginn, // Achtung: DateTime? (im DTO auch so lassen)
                Tipptermin = s.Spiel.SpielBeginn, // Gleiches
                ToreHeim = s.Spiel.TeamAScore,
                ToreGast = s.Spiel.TeamBScore,
                DeinGewinn = _context.Tippscheine
                                .Where(ts => ts.SpielId == s.Spiel.Id && ts.UserId == userId)
                                .Select(ts => ts.Points)
                                .FirstOrDefault(),
                Tipp = new TippDto
                {
                    HeimTore = _context.Tippscheine
                                  .Where(ts => ts.SpielId == s.Spiel.Id && ts.UserId == userId)
                                  .Select(ts => ts.TippA)
                                  .FirstOrDefault() , // nur wenn TippA int? ist
                    GastTore = _context.Tippscheine
                                  .Where(ts => ts.SpielId == s.Spiel.Id && ts.UserId == userId)
                                  .Select(ts => ts.TippB)
                                  .FirstOrDefault()  // nur wenn TippB int? ist
                }
            }).ToList();

        return Ok(spiele);
    }

    /// <summary>
    /// Liefert eine Liste von Spielen, für die der User noch keinen Tipp abgegeben hat.
    /// </summary>
    /// <param name="userId">User ID</param>
    [HttpGet("GetMissingTipp")]
    [AllowAnonymous]
    public ActionResult<List<TippErinnerungDto>> GetMissingTipp(int userId)
    {
        var missingTipps = _context.SpieleInTippgruppen
            .Include(s => s.Spiel)
            .ThenInclude(s => s.TeamA)
            .Include(s => s.Spiel)
            .ThenInclude(s => s.TeamB)
            .Where(s => s.Spiel.SpielBeginn > DateTime.Now) // Nur zukünftige Spiele
            .Where(s => !_context.Tippscheine.Any(ts => ts.UserId == userId && ts.SpielId == s.SpielId)) // Keine Tipps für das Spiel
            .Select(s => new TippErinnerungDto
            {
                SpielBezeichnung = $"{s.Spiel.TeamA.Name} vs {s.Spiel.TeamB.Name}",
                TippgruppeName = s.Tippgruppe.Name
            })
            .ToList();

        // Gruppiere nach Gruppe und finde die mit den meisten fehlenden Tipps
        var gruppeMitMeistenFehlenden = missingTipps
            .GroupBy(x => x.TippgruppeName)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        if (gruppeMitMeistenFehlenden == null)
            return Ok(new List<TippErinnerungDto>());

        // Gib nur die Spiele dieser Gruppe zurück
        return Ok(gruppeMitMeistenFehlenden.ToList());
    }
    /// <summary>
    /// Liefert eine Liste von Spielen, die bald beginnen und für die der User Tipp abgegeben hat.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns></returns>
    [HttpGet("GetUpcomingBetGames")]
    [AllowAnonymous]
    public ActionResult<List<TippErinnerungDto>> GetUpcomingBetGames(int userId)
    {
        var now = DateTime.Now;

        var upcomingGames = _context.SpieleInTippgruppen
            .Include(s => s.Spiel)
                .ThenInclude(s => s.TeamA)
            .Include(s => s.Spiel)
                .ThenInclude(s => s.TeamB)
            .Where(s => _context.Tippscheine.Any(ts => ts.UserId == userId && ts.SpielId == s.SpielId))
            .Where(s => s.Spiel.SpielBeginn > now)
            .Select(s => new TippErinnerungDto
            {
                SpielBezeichnung = $"{s.Spiel.TeamA.Name} vs {s.Spiel.TeamB.Name}",
                TippgruppeName = s.Tippgruppe.Name,
                Datum = s.Spiel.SpielBeginn
            })
            .OrderBy(s => s.Datum)
            .ToList();

        return Ok(upcomingGames);
    }


}
