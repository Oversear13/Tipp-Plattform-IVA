using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TippPlattform.Models;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TippPlattform.Controllers;
/// <summary>
/// Controller für die Startseite und allgemeine Seiten der Tipp-Plattform.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly TippPlattformContext _context;

    public HomeController(ILogger<HomeController> logger, TippPlattformContext context)
    {
        _logger = logger;
        _context = context;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        var offizielleGruppe = _context.Tippgruppen
            .Where(x => !string.IsNullOrEmpty(x.Badge))
            .Include(x=>x.Beitrittes)
            .Include(x => x.Sporttype)
            .Include(x => x.SpieleInTippgruppes)
            .ToList();
        return View(offizielleGruppe);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult AboutUs()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Fragen()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Impressum()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Datenschutz()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Agb()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }
    /// <summary>
    /// Authentifiziert einen Benutzer basierend auf den Anmeldedaten und erstellt eine Authentifizierungssitzung.
    /// </summary>
    /// <param name="model">Username & Passwort</param>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password); // Ideally, use hashed passwords!

            if (user != null)
            {
                // Create user claims
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "user"),
                new Claim("UserId", user.Id.ToString())
            };

                // Create identity and principal
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Check if user has unread messages and save in viewdata to show in _layout
                ViewData["HasUnread"] = _context.Nachrichten.Any(n => n.EmpfaengerId == user.Id && n.GelesenDatum == null);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Ungültiger Benutzername oder Passwort.");
        }

        return View(model);
    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Home");
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Anmelden()
    {
        return View();
    }
    /// <summary>
    /// Registriert einen neuen Benutzer mit den angegebenen Anmeldedaten und speichert ihn in der Datenbank.
    /// </summary>
    /// <param name="model">Anmelden-Daten: username, email, passwort, etc</param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public IActionResult Anmelden(AnmeldenViewModel model)
    {
        var userExisted = _context.Users.Any(x => x.Username == model.Username || x.Email == model.Email);
        if (userExisted)
        {
            // Add a model state error with a message
            ModelState.AddModelError(string.Empty, "Benutzername oder E-Mail ist bereits vergeben.");
            return View(model);  // Return the view with the model to display the error message
        }
        var newUser = new User
        {
            Username = model.Username,
            Password = model.Password,
            Role = "User",
            Email = model.Email,
            Geburtstag = model.Geburtsdatum,
            CreatedAt = DateTime.Now
        };
        _context.Users.Add(newUser);
        _context.SaveChanges();
        return RedirectToAction("Login", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
