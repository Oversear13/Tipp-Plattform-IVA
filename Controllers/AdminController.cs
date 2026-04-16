using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TippPlattform.Data;
using TippPlattform.DTOs;
using TippPlattform.Models;
using TippPlattform.Services;

namespace TippPlattform.Controllers
{
    /// <summary>
    /// Controller für die Admin-Funktionen der Tipp-Plattform.
    /// Verwaltet Spiele, Benutzer und Tippgruppen.
    /// </summary>
    public class AdminController : Controller
    {
        private readonly FootballDataService _footballService;
        private readonly TippPlattformContext _tippPlattformContext;
        private readonly TippPlattformService _tippPlattformService;

        public AdminController(FootballDataService footballService, TippPlattformContext tippPlattformContext, TippPlattformService tippPlattformService)
        {
            _footballService = footballService;
            _tippPlattformContext = tippPlattformContext;
            _tippPlattformService = tippPlattformService;
        }

        /// <summary>
        /// Admin-Dashboard-Seite: Übersicht über aktuelle und bevorstehende Spiele sowie Ergebnisse
        /// Hier kann man Spiele automatisch aus der API laden.
        /// </summary>
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var vm = new AdminViewModel()
            {
                Spielergebnisse = _tippPlattformContext.Spiele
                    .Include(x => x.TeamA)
                    .Include(x => x.TeamB)
                    .Where(x => x.TeamAScore != null && x.TeamBScore != null)
                    .OrderByDescending(x => x.SpielBeginn)
                    .ToList(),

                AusstehendeSpielergebnisse = _tippPlattformContext.Spiele
                    .Include(x => x.TeamA)
                    .Include(x => x.TeamB)
                    .Where(x => x.SpielBeginn != null && x.SpielBeginn.Value.AddDays(1) < DateTime.Now
                        && (x.TeamAScore == null || x.TeamBScore == null))
                    .OrderBy(x => x.SpielBeginn)
                    .ToList(),

                BevorstehendeSpiele = _tippPlattformContext.Spiele
                    .Include(x => x.TeamA)
                    .Include(x => x.TeamB)
                    .Where(x => x.SpielBeginn > DateTime.Now)
                    .OrderBy(x => x.SpielBeginn)
                    .ToList(),

                Date = DateTime.Now
            };
            return View(vm);
        }
        /// <summary>
        /// Zeigt eine Liste aller Spiele in der Plattform an.
        /// Hier kann man manuell Spiele hinzufügen, bearbeiten oder löschen.
        /// </summary>
        [Authorize(Roles = "Admin")]
        public IActionResult SpieleListe()
        {
            var vm = new SpileListeViewModel()
            {
                Ligen = _tippPlattformContext.Ligen
                    .Include(x => x.Spieles)
                    .ThenInclude(x => x.TeamA)
                    .Include(x => x.Spieles)
                    .ThenInclude(x => x.TeamB)
                    .ToList()
            };
            return View(vm);
        }

        /// <summary>
        /// Holt Fußballspiele von einer externen API und speichert sie in der Datenbank.
        /// 1. Manschafsdaten speichern
        /// 2. Spiele speichern
        /// </summary>
        /// <param name="model">Suchparameter für die API-Abfrage (Datum)</param>
        /// <returns>Redirect zur Index-Seite nach erfolgreichem Speichern und Punktevergabe</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SaveMatchesToDatabase(SaveMatchesToDatabaseVM model)
        {
            // API-Daten abrufen
            var matchesJson = await _footballService.GetMatchesAsync(model);
            // Convertiere JSON in C#-Objekte
            var apiData = JsonSerializer.Deserialize<APIFootballResponse>(matchesJson);

            foreach (var match in apiData.Matches)
            {
                var homeTeam = await _tippPlattformContext.Mannschaften
                    .FirstOrDefaultAsync(x => x.APIMannschaftID == match.HomeTeam.TeamId);

                // Wenn die Mannschaft nicht existiert, anlegen
                if (homeTeam == null)
                {
                    homeTeam = new Mannschaft
                    {
                        APIMannschaftID = match.HomeTeam.TeamId,
                        Name = match.HomeTeam.TeamName,
                        LigaId = match.League.LeagueId
                    };
                    _tippPlattformContext.Mannschaften.Add(homeTeam);
                }

                await _tippPlattformContext.SaveChangesAsync();

                var awayTeam = await _tippPlattformContext.Mannschaften
                    .FirstOrDefaultAsync(x => x.APIMannschaftID == match.AwayTeam.TeamId);

                // Wenn die Mannschaft nicht existiert, anlegen
                if (awayTeam == null)
                {
                    awayTeam = new Mannschaft
                    {
                        APIMannschaftID = match.AwayTeam.TeamId,
                        Name = match.AwayTeam.TeamName,
                        LigaId = match.League.LeagueId
                    };
                    _tippPlattformContext.Mannschaften.Add(awayTeam);
                }

                await _tippPlattformContext.SaveChangesAsync();

                var spiel = _tippPlattformContext.Spiele.FirstOrDefault(x => x.APISpielID == match.MatchID);
                // Wenn das Spiel nicht existiert, anlegen
                if (spiel == null)
                {
                    _tippPlattformContext.Spiele.Add(new Spiele
                    {
                        APISpielID = match.MatchID,
                        TeamAId = homeTeam.Id,
                        TeamBId = awayTeam.Id,
                        TeamAScore = match.Scores?.ApiScoreFulltime?.HomeScore,
                        TeamBScore = match.Scores?.ApiScoreFulltime?.AwayScore,
                        SpielBeginn = match.UtcDate,
                        LigaId = match.League.LeagueId,
                        Spieltag = match.Matchday
                    });
                }
                // Wenn das Spiel existiert, aktualisieren
                else
                {
                    if (match.Scores?.ApiScoreFulltime?.HomeScore != null &&
                        match.Scores?.ApiScoreFulltime?.AwayScore != null)
                    {
                        spiel.TeamAScore = match.Scores.ApiScoreFulltime.HomeScore;
                        spiel.TeamBScore = match.Scores.ApiScoreFulltime.AwayScore;
                    }
                }

                await _tippPlattformContext.SaveChangesAsync();
            }
            // Punkte für abgeschlossene Spiele auszahlen
            await PayOutPoints();
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Berechnet und zahlt Punkte für alle abgeschlossenen Tippscheine aus
        /// </summary>
        private async Task PayOutPoints()
        {
            var completedTippscheine = _tippPlattformContext.Tippscheine
                .Include(t => t.Spiel)
                .Where(t => t.Spiel.TeamAScore != null && t.Spiel.TeamBScore != null &&
                            t.PaidOut == null)
                .ToList();

            foreach (var tippschein in completedTippscheine)
            {
                int points = _tippPlattformService.CalculatePoints(tippschein);

                var user = await _tippPlattformContext.Beitritte
                    .FirstOrDefaultAsync(u => u.UserId == tippschein.UserId && u.TippgruppeId == tippschein.TippgruppeId);

                if (user != null)
                {
                    user.Points += points;
                    tippschein.Points = points;
                    tippschein.PaidOut = DateTime.Now;
                }
            }

            await _tippPlattformContext.SaveChangesAsync();
        }

        [HttpGet("api/teams")]
        public IActionResult GetTeamsByLeague(int leagueId)
        {
            var teams = _tippPlattformContext.Mannschaften
                .Where(t => t.LigaId == leagueId)
                .Select(t => new { t.Id, t.Name })
                .ToList();

            return Json(teams);
        }

        // Admin can manually add a match
        [Authorize(Roles = "Admin")]
        public IActionResult SpielHinzufuegen(int? liga, int? teamAId, int? teamBId, DateTime? datum, TimeOnly? uhrzeit, int? spieltag)
        {
            var teamA = _tippPlattformContext.Mannschaften
                .Any(t => t.Id == teamAId);
            var teamB = _tippPlattformContext.Mannschaften
                .Any(t => t.Id == teamBId);
            if (teamA && teamB)
            {
                var newGame = new Spiele
                {
                    TeamAId = teamAId.Value,
                    TeamBId = teamBId.Value,
                    SpielBeginn = datum?.Add(uhrzeit?.ToTimeSpan() ?? TimeSpan.Zero),
                    LigaId = liga,
                    Spieltag = spieltag,
                };
                _tippPlattformContext.Spiele.Add(newGame);
                _tippPlattformContext.SaveChanges();
                return RedirectToAction("SpieleListe");
            }
            return RedirectToAction("SpieleListe");
        }
        // Admin can edit a game manually
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult SpielBearbeiten(int id, int teamAId, int teamBId, int? scoreA, int? scoreB, DateTime datum, TimeOnly uhrzeit, int spieltag)
        {
            var spiel = _tippPlattformContext.Spiele.FirstOrDefault(s => s.Id == id);
            if (spiel != null)
            {
                spiel.TeamAId = teamAId;
                spiel.TeamBId = teamBId;
                spiel.SpielBeginn = datum.Add(uhrzeit.ToTimeSpan());
                spiel.TeamAScore = scoreA;
                spiel.TeamBScore = scoreB;
                _tippPlattformContext.SaveChanges();
            }
            return RedirectToAction("SpieleListe");
        }
        // Admin can delete a game manually
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult SpielLoeschen(int spielId)
        {
            var spiel = _tippPlattformContext.Spiele.FirstOrDefault(s => s.Id == spielId);
            if (spiel != null)
            {
                _tippPlattformContext.Spiele.Remove(spiel);
                _tippPlattformContext.SaveChanges();
            }
            return RedirectToAction("SpieleListe");
        }
        // ---------------- BENUTZER-VERWALTUNG ----------------

        [Authorize(Roles = "Admin")]
        public IActionResult UserListe(string searchString) // HIER: searchString als Parameter hinzugefügt
        {
            var users = from u in _tippPlattformContext.Users
                        select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.Username.Contains(searchString));
            }

            return View(users.ToList());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditUser(int id)
        {
            var user = _tippPlattformContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditUser(User user)
        {
            var dbUser = _tippPlattformContext.Users.FirstOrDefault(u => u.Id == user.Id);
            if (dbUser == null)
                return NotFound();

            dbUser.Username = user.Username;
            dbUser.Email = user.Email;
            dbUser.Role = user.Role;

            _tippPlattformContext.SaveChanges();

            return RedirectToAction("UserListe");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int id)
        {
            var user = _tippPlattformContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            // Zuerst alle Nachrichten löschen, in denen der User Empfänger ist
            var empfangeneNachrichten = _tippPlattformContext.Nachrichten
                .Where(n => n.EmpfaengerId == id);
            _tippPlattformContext.Nachrichten.RemoveRange(empfangeneNachrichten);

            // Dann alle Nachrichten löschen, in denen der User Sender ist
            var gesendeteNachrichten = _tippPlattformContext.Nachrichten
                .Where(n => n.SenderId == id);
            _tippPlattformContext.Nachrichten.RemoveRange(gesendeteNachrichten);

            // Jetzt den User löschen
            _tippPlattformContext.Users.Remove(user);

            _tippPlattformContext.SaveChanges();

            return RedirectToAction("UserListe");
        }


        // ---------------- TIPPGRUPPEN-VERWALTUNG ----------------
        [Authorize(Roles = "Admin")]
        public IActionResult TippgruppenListe()
        {
            var tippGruppen = _tippPlattformContext.Tippgruppen
                .Include(tg => tg.Sporttype)
                .Include(tg => tg.TippgruppeAdmins)
                .Select(tg => new TippgruppenVerwaltung
                {
                    Id = tg.Id,
                    Name = tg.Name ?? "",
                    Beschreibung = tg.Beschreibung ?? "",
                    SportArt = tg.Sporttype.Name ?? "",
                    Gruppenbesitzer = tg.TippgruppeAdmins
                        .Select(a => a.User.Username)
                        .FirstOrDefault() ?? "",
                    AnzalMitglieder = _tippPlattformContext.Beitritte
                        .Where(x => x.TippgruppeId == tg.Id)
                        .Count(),
                    Badge = tg.Badge // Badge for the official Tippgruppe
                })
                .ToList();
            return View(tippGruppen);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteGroup(int id)
        {
            // 1. Abhängige Einträge löschen

            // SpieleInTippgruppen mit dieser Tippgruppe löschen
            var spieleInTippgruppe = _tippPlattformContext.SpieleInTippgruppen
                .Where(x => x.TippgruppeId == id);
            _tippPlattformContext.SpieleInTippgruppen.RemoveRange(spieleInTippgruppe);

            // Beitritte mit dieser Tippgruppe löschen
            var beitritte = _tippPlattformContext.Beitritte
                .Where(x => x.TippgruppeId == id);
            _tippPlattformContext.Beitritte.RemoveRange(beitritte);

            // Tippscheine mit dieser Tippgruppe löschen
            var tippscheine = _tippPlattformContext.Tippscheine
                .Where(x => x.TippgruppeId == id);
            _tippPlattformContext.Tippscheine.RemoveRange(tippscheine);

            // Tippgruppen-Admins mit dieser Tippgruppe löschen
            var admins = _tippPlattformContext.TippgruppeAdmins
                .Where(x => x.TippgruppeId == id);
            _tippPlattformContext.TippgruppeAdmins.RemoveRange(admins);

            // PunkteRegeln mit dieser Tippgruppe löschen (achte auf Property Tippgruppe_Id)
            var punkteRegeln = _tippPlattformContext.PunkteRegeln
                .Where(x => x.Tippgruppe_Id == id);
            _tippPlattformContext.PunkteRegeln.RemoveRange(punkteRegeln);

            // 2. Die Tippgruppe selbst löschen
            var gruppe = _tippPlattformContext.Tippgruppen.FirstOrDefault(x => x.Id == id);
            if (gruppe != null)
            {
                _tippPlattformContext.Tippgruppen.Remove(gruppe);
            }

            // 3. Änderungen speichern
            _tippPlattformContext.SaveChanges();

            return RedirectToAction("TippgruppenListe");
        }

    }
}