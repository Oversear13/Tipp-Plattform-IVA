using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TippPlattform.Models;
using TippPlattform.Models.SeedData;

namespace TippPlattform.Controllers
{
    public class TippGruppenController : Controller
    {
        private readonly TippPlattformContext _context;
        public TippGruppenController(TippPlattformContext context)
        {
            _context = context;
        }
		// Gruppenübersicht (Index action to display the list of Tippgruppen that the user has joined or is an group-admin.)

		public IActionResult Tippen(int gruppeId)
		{
			var gruppe = _context.Tippgruppen
                .Include(g=>g.TippgruppeAdmins)
				.FirstOrDefault(g => g.Id == gruppeId);

			var beitritte = _context.Beitritte
				.Where(b => b.TippgruppeId == gruppeId)
				.Include(b => b.User)
				.ToList();

			var viewModel = new TippenViewModel(gruppe)
			{
				Beitritte = beitritte
			};

			ViewBag.GruppeId = gruppeId; //neu


			return View(viewModel);
		}

		public IActionResult Index()
        {
            // Get User Id from Claims
            var userId = int.Parse(User.FindFirst("UserId")?.Value!);

            var viewModel = new TippGruppenViewModel
            {
                TippGruppen = _context.Beitritte
                                .Include(tg => tg.Tippgruppe)
                                .ThenInclude(tg => tg.Sporttype)
                                .Include(tg => tg.Tippgruppe)
                                .ThenInclude(tg=>tg.TippgruppeAdmins)
                                .Where(b => b.UserId == userId)
                                .Select(tg => new TippGruppe
                                {
                                    Id = tg.TippgruppeId,
                                    Name = tg.Tippgruppe.Name,
                                    Beschreibung = tg.Tippgruppe.Beschreibung,
                                    CreatedAt = tg.Tippgruppe.CreatedAt,
                                    SportArt = tg.Tippgruppe.Sporttype.Name ?? "",
                                    AnzahlSpiele = _context.SpieleInTippgruppen // Number of games that can be tipped
                                    .Include(x => x.Spiel)
                                        .Where(x => x.TippgruppeId == tg.TippgruppeId && x.Spiel.SpielBeginn > DateTime.Now).Count(),
                                    Tipptermin = _context.SpieleInTippgruppen
                                        .Include(s => s.Spiel)
                                        .Where(s => s.TippgruppeId == tg.TippgruppeId
                                        && s.Spiel.TeamAScore == null)
                                        .Select(s => s.Spiel.SpielBeginn)
                                        .OrderBy(d => d)
                                        .FirstOrDefault(),
                                    IstAdmin = tg.Tippgruppe.TippgruppeAdmins.Any(a => a.UserId == userId), // Check if the user is an admin of the group
                                    Badge = tg.Tippgruppe.Badge // Badge for the official Tippgruppe
                                })
                                .ToList()
            };
            return View(viewModel);
        }
        // Display page to create a new Tippgruppe.
        [HttpGet]
        public IActionResult TippGruppeErstellen()
        {
            var model = new CreateTippGruppeViewModel
            {
                VerfügbareSportarten = _context.Sporttypen
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult TippGruppeErstellen(CreateTippGruppeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.VerfügbareSportarten = _context.Sporttypen
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToList();
                return View(model);
            }

            var userId = int.Parse(User.FindFirst("UserId")?.Value!);

            var newTippGruppe = new Tippgruppe
            {
                Name = model.Name,
                Beschreibung = model.Beschreibung,
                SporttypeId = model.SporttypeId,
                Passwort = model.Passwort,
                Badge = model.Badge,
                CreatedAt = DateTime.Now
            };
            _context.Tippgruppen.Add(newTippGruppe);
            _context.SaveChanges();

            _context.TippgruppeAdmins.Add(new TippgruppeAdmin
            {
                TippgruppeId = newTippGruppe.Id,
                UserId = userId
            });

            _context.Beitritte.Add(new Beitritt
            {
                TippgruppeId = newTippGruppe.Id,
                UserId = userId,
                JoinedAt = DateTime.Now,
                Points = 0
            });

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult GruppenSuchen(string Name)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value!);

            var tippGruppen = _context.Tippgruppen
                .Include(x => x.Sporttype)
                .Include(x => x.Beitrittes) // include user relations
                .Where(x => x.Name != null && x.Name.Contains(Name))
                .Where(x => !x.Beitrittes.Any(tu => tu.UserId == userId)) // exclude groups where user is already a member
                .ToList();

            var ViewModel = new TippgruppenSuchenViewModel
            {
                TippGruppen = tippGruppen
            };
            return View("Beitreten", ViewModel);
        }

        // Function to join a Tippgruppe.
        [HttpPost]
		public IActionResult Beitreten(int gruppeId, string? passwort)
		{
			var userId = int.Parse(User.FindFirst("UserId")?.Value!);
			var gruppe = _context.Tippgruppen.FirstOrDefault(tg => tg.Id == gruppeId);

			if (gruppe == null)
			{
				return NotFound();
			}
			if (gruppe.Passwort != null && gruppe.Passwort != passwort)
            {
                TempData["ErrorMessage"] = "Falsches Passwort.";
                return RedirectToAction("GruppenSuchen", new { Name = gruppe.Name });
			}

            // Check if the user is already a member of the group
            // If not, add them to the group
            if (!_context.Beitritte.Any(b => b.TippgruppeId == gruppeId && b.UserId == userId))
			{
				_context.Beitritte.Add(new Beitritt
				{
					TippgruppeId = gruppeId,
					UserId = userId,
					JoinedAt = DateTime.Now
				});
				_context.SaveChanges();
			}

			return RedirectToAction("Index");
		}

        // Display page of a specific Tippgruppe. Where user can see the details of the Tippgruppe and the games in it.


        //// Method to invite people as an admin or owner of a group. takes gruppeID and the namen and returns a redirect to stay in the same tab.
        [HttpPost]
		public IActionResult MitgliedEinladen(int gruppeId, string userIdentifier)
		{
			var user = _context.Users
				.FirstOrDefault(u => u.Username == userIdentifier || u.Id.ToString() == userIdentifier);

			if (user == null)
			{
				TempData["ModalMessage"] = "Benutzer konnte nicht gefunden werden.";
				return RedirectToAction("Tippen", new { gruppeId });
			}

			var istSchonMitglied = _context.Beitritte
				.Any(b => b.UserId == user.Id && b.TippgruppeId == gruppeId);

			if (istSchonMitglied)
			{
				TempData["ModalMessage"] = "Benutzer ist bereits Mitglied.";
				return RedirectToAction("Tippen", new { gruppeId });
			}

			_context.Nachrichten.Add(new Nachricht
			{
                SenderId = int.Parse(User.FindFirst("UserId")?.Value!),
                EmpfaengerId = user.Id,
                Nachrichtentext = $"Hey! Du wurdest in die Tippgruppe '{_context.Tippgruppen.FirstOrDefault(g => g.Id == gruppeId)?.Name}' eingeladen.",
                SendDatum = DateTime.Now,
                GelesenDatum = null,
                EingeladeneGruppeId = gruppeId // Set the group ID for the invitation

            });
			_context.SaveChanges();

			TempData["ModalMessage"] = "Benutzer wurde erfolgreich eingeladen.";
			return RedirectToAction("Tippen", new { gruppeId, tab = "tab4" });  //geändert
		}

        // // Method to delete úsers from a group as an admin or owner of a group. Takes the groupID and userID and return a redirect to
        //stay in the same tab.
        [HttpPost]
		public IActionResult MitgliedEntfernen(int gruppeId, int userId)
		{
			var beitritt = _context.Beitritte
				.FirstOrDefault(b => b.TippgruppeId == gruppeId && b.UserId == userId);

			if (beitritt != null)
			{
				_context.Beitritte.Remove(beitritt);
				_context.SaveChanges();
				TempData["ModalMessage"] = "Mitglied erfolgreich entfernt.";
			}
			else
			{
				TempData["ModalMessage"] = "Es ist leider ein Fehler aufgetreten.";
			}

			return RedirectToAction("Tippen", new { gruppeId, tab = "tab4" }); //geändert
		}

        //// Method to leave a group. takes the groupID and returns a redirect.


        [HttpPost]
		public IActionResult GruppeVerlassen(int gruppeId)
		{
			var currentUserId = int.Parse(User.FindFirst("UserId").Value);

			var beitritt = _context.Beitritte
				.FirstOrDefault(b => b.TippgruppeId == gruppeId && b.UserId == currentUserId);

			if (beitritt != null)
			{
				_context.Beitritte.Remove(beitritt);
				_context.SaveChanges();
				TempData["SuccessMessage"] = "Du hast die Gruppe erfolgreich verlassen.";
				return RedirectToAction("Index", "TippGruppen");
			}

			TempData["ErrorMessage"] = "Du bist kein Mitglied dieser Gruppe.";
			return RedirectToAction("Tippen", new { gruppeId });
		}




	}
}
