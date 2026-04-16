using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TippPlattform.Models;
using TippPlattformMaui.Shared.Dtos;

namespace TippPlattform.Controllers.Api
{
    /// <summary>
    /// Schnittstelle für App und Datenbank durch eine Api-Anbindung
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TippgruppenApiController : ControllerBase
    {
        private readonly TippPlattformContext _context;

        public TippgruppenApiController(TippPlattformContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Liefert alle Tippgruppen, denen der angegebene Benutzer beigetreten ist.
        /// </summary>
        /// <param name="userId">ID des Benutzers</param>
        /// <returns>Eine Liste von <see cref="TippGruppeDto"/>-Objekten mit Informationen zur jeweiligen Tippgruppe.</returns>
        [HttpGet("meine")]
        [AllowAnonymous]
        public ActionResult<List<TippGruppeDto>> GetMeineTippgruppen(int userId)
        {
            var gruppen = _context.Beitritte
                .Include(t => t.Tippgruppe).ThenInclude(x => x.Sporttype)
                .Include(t => t.Tippgruppe).ThenInclude(x => x.TippgruppeAdmins)
                .Where(b => b.UserId == userId)
                .Select(tg => new TippGruppeDto
                {
                    Id = tg.TippgruppeId,
                    Name = tg.Tippgruppe.Name,
                    SportArt = tg.Tippgruppe.Sporttype.Name,
                    AnzahlSpiele = _context.SpieleInTippgruppen
                        .Include(s => s.Spiel)
                        .Count(x => x.TippgruppeId == tg.TippgruppeId && x.Spiel.SpielBeginn > DateTime.Now),
                    Tipptermin = _context.SpieleInTippgruppen
                        .Include(s => s.Spiel)
                        .Where(x => x.TippgruppeId == tg.TippgruppeId && x.Spiel.TeamAScore == null)
                        .OrderBy(x => x.Spiel.SpielBeginn)
                        .Select(x => x.Spiel.SpielBeginn)
                        .FirstOrDefault(),
                    IstAdmin = tg.Tippgruppe.TippgruppeAdmins.Any(a => a.UserId == userId)
                })
                .ToList();

            return gruppen;
        }

        /// <summary>
        /// Sucht nach Tippgruppen anhand eines Namensfragments.
        /// </summary>
        /// <param name="name">Name der gesuchten Tippgruppe</param>
        /// <param name="userId">Die ID des aktuell angemeldeten Benutzers (zur Admin-Erkennung)</param>
        /// <returns>
        /// Eine Liste von <see cref="TippGruppeDto"/>-Objekten, die zur Suchanfrage passen.
        /// </returns>
        [HttpGet("search")]
        [AllowAnonymous]
        public ActionResult<List<TippGruppeDto>> SearchTippgruppen(
            [FromQuery] string name,
            [FromQuery] int userId)
        {
            var gruppen = _context.Tippgruppen
                .Include(g => g.Sporttype)
                .Include(g => g.TippgruppeAdmins)
                .Where(g => g.Name.Contains(name))
                .Select(g => new TippGruppeDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    SportArt = g.Sporttype.Name,
                    AnzahlSpiele = _context.SpieleInTippgruppen
                        .Count(x => x.TippgruppeId == g.Id && x.Spiel.SpielBeginn > DateTime.Now),
                    Tipptermin = _context.SpieleInTippgruppen
                        .Where(x => x.TippgruppeId == g.Id && x.Spiel.TeamAScore == null)
                        .OrderBy(x => x.Spiel.SpielBeginn)
                        .Select(x => x.Spiel.SpielBeginn)
                        .FirstOrDefault(),
                    // Hier der Check, ob der eingeloggte User Admin dieser Gruppe ist:
                    IstAdmin = _context.TippgruppeAdmins
                        .Any(a => a.UserId == userId && a.TippgruppeId == g.Id)
                })
                .ToList();

            return gruppen;
        }

    }
}
