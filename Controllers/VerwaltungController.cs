using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TippPlattform.Models;

namespace TippPlattform.Controllers
{
    /// <summary>
    /// Partial View Controller for managing A Tippgruppe
    /// </summary>
    public class VerwaltungController : Controller
    {
        private readonly TippPlattformContext _context;
        public VerwaltungController(TippPlattformContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }// A page to manange the Tippgruppe. (Add games, Change group name, etc.)
        public IActionResult Verwaltung(int? gruppeId)
        {
            var TippGruppe = _context.Tippgruppen
                .Include(x => x.TippgruppeAdmins)
                .FirstOrDefault(x => x.Id == gruppeId); // Get the Tippgruppe by Id
            var spieleInGruppe = _context.SpieleInTippgruppen
                .Where(x => x.TippgruppeId == gruppeId)
                .Include(x=>x.Spiel)
                    .ThenInclude(sp => sp.TeamA)
                .Include(x => x.Spiel)
                    .ThenInclude(sp => sp.TeamB)
                .Include(x=>x.PunkteRegel)
                .OrderByDescending(x=>x.Spiel.SpielBeginn)
                .ToList();
            var punkteRegeln = _context.PunkteRegeln
                .Where(x => x.Tippgruppe_Id == gruppeId)
                .ToList();
            var viewModel = new VerwaltungViewModel()
            {
                Tippgruppe = TippGruppe,
                IstMitPasswort = !string.IsNullOrEmpty(TippGruppe?.Passwort),
                PunkteRegeln = punkteRegeln,
                PunkteRegelnSelectList = punkteRegeln.Select(x=>new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList(),
                SpieleInGruppe = spieleInGruppe,
                AlleSpiele = _context.Spiele // list of all games that can be added into group
                    .Include(s => s.TeamA)
                    .Include(s => s.TeamB)
                    .Where(s => s.SpielBeginn > DateTime.Now
                        && s.TeamAScore == null
                        && !spieleInGruppe.Select(x => x.SpielId).Contains(s.Id))
                    .OrderBy(s => s.SpielBeginn)
                    .ToList(),
                AlleLigen = _context.Ligen.ToList()
            };
            return PartialView("_Verwaltung", viewModel); // Return the partial view with data
        }
        // Function to add a game to a Tippgruppe.
        public IActionResult SpielHinzufuegen(int? groupId, int? spielId, int? regelId)
        {
            var spiel = _context.Spiele.FirstOrDefault(s => s.Id == spielId);
            var tippGruppe = _context.Tippgruppen.FirstOrDefault(tg => tg.Id == groupId);
            var regelSet = _context.PunkteRegeln.FirstOrDefault(r => r.Id == regelId);
            if (spiel != null && tippGruppe != null)
            {
                if (!_context.SpieleInTippgruppen.Any(s => s.SpielId == spiel.Id && s.TippgruppeId == tippGruppe.Id))
                {
                    var spielInTippGruppe = new SpieleInTippgruppe
                    {
                        TippgruppeId = tippGruppe.Id,
                        SpielId = spiel.Id,
                        PunkteRegelId = regelSet != null ? regelSet.Id : null,
                    }
                ;
                    _context.SpieleInTippgruppen.Add(spielInTippGruppe);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Tippen", "TippGruppen", new { gruppeId = groupId , tab = "tab5" });
        }
        // Function to delete a game from a Tippgruppe.
        public IActionResult SpielInGruppeLoeschen(int? groupId, int? spielId)
        {
            var spielInGruppe = _context.SpieleInTippgruppen.FirstOrDefault(x => x.Id == spielId);
            if (spielInGruppe != null)
            {
                var tippScheine = _context.Tippscheine
                    .Where(x => x.SpielId == spielInGruppe.SpielId)
                    .ToList();
                _context.Tippscheine.RemoveRange(tippScheine);
                _context.SpieleInTippgruppen.Remove(spielInGruppe);
                _context.SaveChanges();
            }
            return RedirectToAction("Tippen", "TippGruppen", new { gruppeId = groupId , tab = "tab5" });
        }
        // Function to edit a game in a Tippgruppe.
        public IActionResult SpielInGruppeBearbeiten(int? groupId, int? spielId, int? regelId)
        {
            var spielInGruppe = _context.SpieleInTippgruppen.FirstOrDefault(x => x.SpielId == spielId);
            var regel = _context.PunkteRegeln.FirstOrDefault(x => x.Id == regelId);
            if(spielInGruppe != null && (regel != null || regelId == 0))
            {
                spielInGruppe.PunkteRegelId = regelId == 0 ? null : regelId;
                _context.SaveChanges();
            }
            return RedirectToAction("Tippen", "TippGruppen", new { gruppeId = groupId });
        }
        // Function to add a new PunkteRegel to a Tippgruppe.
        public IActionResult RegelHinzufuegen(int? groupId, VerwaltungViewModel model)
        {
            if (model.NewPunkteRegel != null && groupId != null)
            {
                var newRegel = new PunkteRegel
                {
                    Name = model.NewPunkteRegel.Name,
                    Beschreibung = model.NewPunkteRegel.Beschreibung,
                    Quote1 = model.NewPunkteRegel.Quote1,
                    Quote2 = model.NewPunkteRegel.Quote2,
                    Quote3 = model.NewPunkteRegel.Quote3,
                    Quote4 = model.NewPunkteRegel.Quote4,
                    Tippgruppe_Id = groupId.Value
                };
                _context.PunkteRegeln.Add(newRegel);
                _context.SaveChanges();
            }
            return RedirectToAction("Tippen", "TippGruppen", new { gruppeId = groupId , tab = "tab5" });

        }
        // Function to delete a PunkteRegel from a Tippgruppe.
        public IActionResult RegelLoeschen(int? groupId, int? regelId)
        {
            var punkteRegel = _context.PunkteRegeln.FirstOrDefault(x => x.Id == regelId);
            var spieleInGruppe = _context.SpieleInTippgruppen
                .Where(x => x.PunkteRegelId == regelId && x.TippgruppeId == groupId);
            // Manually remove the foreign key constraint
            if (spieleInGruppe != null)
            {
                foreach (var item in spieleInGruppe)
                {
                    item.PunkteRegelId = null;
                }
                _context.SaveChanges();
            }
            if (punkteRegel != null)
            {
                _context.PunkteRegeln.Remove(punkteRegel);
                _context.SaveChanges();
            }
            return RedirectToAction("Tippen", "TippGruppen", new { gruppeId = groupId , tab = "tab5"});
        }
        // Function to edit a PunkteRegel in a Tippgruppe.
        public IActionResult RegelBearbeiten(int? regelId, int quote1, int quote2, int quote3, int quote4)
        {
            var punkteRegel = _context.PunkteRegeln.FirstOrDefault(x => x.Id == regelId);

            if (punkteRegel != null)
            {
                punkteRegel.Quote1 = quote1;
                punkteRegel.Quote2 = quote2;
                punkteRegel.Quote3 = quote3;
                punkteRegel.Quote4 = quote4;
                _context.SaveChanges();
            }
            return RedirectToAction("Tippen", "TippGruppen");
        }
        // Function to save the basic settings of a Tippgruppe. (Name, Beschreibung, Sporttype, Passwort)
        public void GrundeinstellungenSpeichern(VerwaltungViewModel model)
        {
            if (!ModelState.IsValid || model.Tippgruppe == null)
            {
                return;
            }

            var tippgruppe = _context.Tippgruppen.FirstOrDefault(x => x.Id == model.Tippgruppe.Id);
            if(tippgruppe != null)
            {
                tippgruppe.Name = model.Tippgruppe.Name;
                tippgruppe.Beschreibung = model.Tippgruppe.Beschreibung;
                tippgruppe.SporttypeId = model.Tippgruppe.SporttypeId;
                if(model.IstMitPasswort)
                {
                    tippgruppe.Passwort = model.Tippgruppe.Passwort;
                }
                else
                {
                    tippgruppe.Passwort = null; // Remove password if not set
                }
                _context.SaveChanges();
            }
            return;
        }
        // Function to delete a Tippgruppe and all associated games and tips.
        public IActionResult TippgruppeLoeschen(int id)
        {
            var spiele = _context.SpieleInTippgruppen
                .Where(s => s.TippgruppeId == id);
            _context.SpieleInTippgruppen.RemoveRange(spiele); // Prevent Key Constraint Violations

            var tippGruppe = _context.Tippgruppen.FirstOrDefault(x => x.Id == id);
            if (tippGruppe != null)
            {
                _context.Tippgruppen.Remove(tippGruppe);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "TippGruppen");
        }
    }
}
