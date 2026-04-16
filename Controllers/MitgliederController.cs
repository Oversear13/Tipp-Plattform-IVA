using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TippPlattform.Data; // ggf. anpassen
using TippPlattform.Models;

namespace TippPlattform.Controllers
{
    /// <summary>
    /// Partial View MitgliederController verwaltet die Anzeige und Interaktion mit Mitgliedern in einer Tippgruppe.
    /// </summary>
    public class MitgliederController : Controller
	{
		private readonly TippPlattformContext _context;

		public MitgliederController(TippPlattformContext context)
		{
			_context = context;
		}

		public IActionResult MitgliederTab(int gruppeId)
		{
			var mitglieder = _context.Beitritte
				.Where(b => b.TippgruppeId == gruppeId)
				.Include(b => b.User)
				.ToList();

			return PartialView("_MitgliederTab", mitglieder);
		}

		public IActionResult Tippen(int gruppeId)
		{
			var gruppe = _context.Tippgruppen
				.FirstOrDefault(g => g.Id == gruppeId);

			var beitritte = _context.Beitritte
				.Where(b => b.TippgruppeId == gruppeId)
				.Include(b => b.User)
				.ToList();

			var viewModel = new TippenViewModel(gruppe)
			{
				Beitritte = beitritte
			};

			return View(viewModel);
		}

	}
}
