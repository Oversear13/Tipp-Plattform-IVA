using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TippPlattform.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TippPlattform.Controllers
{
    public class UserController : Controller
    {
        private readonly TippPlattformContext _context;

        public UserController(TippPlattformContext context)
        {
            _context = context;
        }

        // --- Admin Actions ---

        [Authorize(Roles = "Admin")]
        public IActionResult UserListe()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult EditUser(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            var existingUser = _context.Users.Find(user.Id);
            if (existingUser == null)
                return NotFound();

            // 👉 Doppelter Benutzername (außer man selbst)
            bool usernameExists = _context.Users
                .Any(u => u.Username == user.Username && u.Id != user.Id);

            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Dieser Benutzername ist bereits vergeben.");

                //⚠️ Dieser Trick ist wichtig: Gib NICHT existingUser zurück, sondern `user`!
                return View(user);
            }

            // 🔄 Werte aktualisieren
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;
            existingUser.Geburtstag = user.Geburtstag;

            _context.SaveChanges();
            return RedirectToAction("UserListe");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("UserListe");
        }

        // --- "Mein Profil" für eingeloggte Nutzer ---

        [Authorize]
        public IActionResult MeinProfil()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Challenge();

            var user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound();

            var viewModel = new MeinProfilViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Geburtstag = user.Geburtstag
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MeinProfil(MeinProfilViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var username = User.Identity?.Name;
            var user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound();

            if (model.Id != user.Id)
                return Forbid();

            // 🔍 Benutzername bereits vergeben (außer vom aktuellen Benutzer)?
            var andererUserMitGleichemNamen = _context.Users
                .FirstOrDefault(u => u.Username == model.Username && u.Id != user.Id);

            if (andererUserMitGleichemNamen != null)
            {
                ModelState.AddModelError("Username", "Der Benutzername ist bereits vergeben.");
                return View(model);
            }

            user.Email = model.Email;
            user.Username = model.Username;
            user.Geburtstag = model.Geburtstag ?? DateTime.MaxValue;

            if (!string.IsNullOrEmpty(model.NeuesPasswort))
            {
                if (model.NeuesPasswort != model.PasswortBestaetigung)
                {
                    ModelState.AddModelError(string.Empty, "Die Passwörter stimmen nicht überein.");
                    return View(model);
                }

                user.Password = model.NeuesPasswort; // 🔐 In echtem Projekt: Passwort hashen!
            }

            await _context.SaveChangesAsync();

            // 🔁 Benutzer neu anmelden (damit User.Identity.Name aktualisiert wird)
            var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, user.Username),
    new Claim(ClaimTypes.Role, user.Role ?? "User"),
    new Claim("UserId", user.Id.ToString()) // 👈 HIER: UserId-Claim hinzufügen
};
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


            ViewBag.Message = "Profil erfolgreich aktualisiert.";
            return View(model);
        }

        // --- NEUE AKTIONEN FÜR DAS LÖSCHEN DES EIGENEN KONTOS ---

        [Authorize]
        [HttpGet]
        public IActionResult DeleteMeinProfilConfirmation()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Challenge(); // Oder Redirect zum Login

            var user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound(); // Benutzer existiert nicht mehr

            // Optional: Ein ViewModel übergeben, das den Benutzernamen enthält
            // um ihn auf der Bestätigungsseite anzuzeigen.
            var viewModel = new MeinProfilViewModel { Username = user.Username, Id = user.Id };
            return View(viewModel); // Eine neue View für die Bestätigung
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteMeinProfilConfirmation")] // Stellt sicher, dass diese POST-Methode der GET-Methode zugeordnet wird
        public async Task<IActionResult> DeleteMeinProfilConfirmed()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Challenge();

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                // Benutzer wurde bereits gelöscht oder nicht gefunden
                return RedirectToAction("Login", "Account"); // Oder eine andere Zielseite
            }

            // ⚠️ WICHTIG: Benutzer ausloggen, BEVOR das Konto gelöscht wird,
            // damit die Session sauber beendet wird.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            // Weiterleitung zu einer Bestätigungsseite oder zur Startseite/Login-Seite
            TempData["SuccessMessage"] = "Ihr Konto wurde erfolgreich gelöscht.";
            return RedirectToAction("Index", "Home"); // Oder eine "Konto gelöscht"-Seite
        }
    }
}
   