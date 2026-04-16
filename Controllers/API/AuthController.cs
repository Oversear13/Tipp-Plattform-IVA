using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TippPlattformMaui.Shared.Dtos;
using TippPlattform.Models;



namespace TippPlattform.Controllers.Api

{
    /// <summary>
    /// Schnittstelle für App und Datenbank durch eine Api-Anbindung
    /// Api Funktionen für Authentifizierung und Registierung 
    /// </summary>
    [ApiController]

    [Route("api/[controller]")]

    [AllowAnonymous]

    public class AuthController : ControllerBase

    {

        private readonly TippPlattformContext _context;
        public AuthController(TippPlattformContext context) => _context = context;

        /// <summary>
        /// Login-Funktion für die App
        /// </summary>
        /// <param name="model">Login Dto von App: username & password</param>
        /// <returns>
        /// 200 OK wenn Anmeldedaten korrekt
        /// 401 Unauthorized wenn Anmeldedaten falsch
        /// </returns>
        [HttpPost("login")]

        [AllowAnonymous]

        public IActionResult Login([FromBody] LoginDto model)

        {

            var user = _context.Users.FirstOrDefault(u =>

                u.Username == model.Username && u.Password == model.Password);


            if (user == null)

                return Unauthorized("Ungültiger Benutzername oder Passwort.");


            return Ok(new

            {

                user.Id,

                user.Username,

                user.Email,

                user.Role

            });

        }
        /// <summary>
        /// Registrieren-Funktion für die App
        /// </summary>
        /// <param name="model">RegisterDto: Username, Passwort, Email, etc</param>
        /// <returns>
        /// 200 Ok wenn Registrierung erfolgreich
        /// 409 Conflict wenn Benutzername oder E-Mail bereits vergeben
        /// </returns>
        [HttpPost("register")]

        [AllowAnonymous]

        public IActionResult Register([FromBody] RegisterDto model)

        {

            if (_context.Users.Any(u => u.Username == model.Username || u.Email == model.Email))

                return Conflict("Benutzername oder E-Mail bereits vergeben.");


            var user = new User

            {

                Username = model.Username,

                Password = model.Password,

                Email = model.Email,

                Role = "User",

                CreatedAt = DateTime.Now

            };


            _context.Users.Add(user);

            _context.SaveChanges();


            return Ok("Registrierung erfolgreich.");

        }
        /// <summary>
        /// Ruft die Benutzerdaten zu einem bestimmten Benutzernamen ab
        /// </summary>
        /// <param name="username">Der Benutzername des gesuchten Benutzers</param>
        /// <returns>
        /// 200 Ok mit Benutzerdaten wenn Benutzer gefunden
        /// 404 NotFound wenn Benutzer nicht gefunden
        /// </returns>
        [HttpGet("{username}")]

        public IActionResult GetUser(string username)

        {

            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)

                return NotFound("Benutzer nicht gefunden.");


            return Ok(new UserResponseDto

            {

                Id = user.Id,

                Username = user.Username,

                Email = user.Email,

                Role = user.Role

            });

        }

        /// <summary>
        /// Aktualisiert die Benutzerdaten eines bestimmten Benutzers
        /// </summary>
        /// <param name="username">Der Benutzername des zu aktualisierenden Benutzers</param>
        /// <param name="model">Benutzerdaten DTO</param>
        /// <returns>
        /// 200 Ok wenn Benutzerdaten aktualisiert wurde
        /// 404 NotFound wenn Benutzer nicht gefunden
        /// </returns>
        [HttpPut("{username}")]

        public IActionResult UpdateUser(string username, [FromBody] RegisterDto model)

        {

            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)

                return NotFound("Benutzer nicht gefunden.");


            user.Email = model.Email;

            user.Password = model.Password;

            user.Geburtstag = model.BirthDate;


            _context.SaveChanges();


            return Ok("Benutzer aktualisiert.");

        }

    }

}