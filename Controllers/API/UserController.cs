// TippPlattform/Controllers/Api/UserApiController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TippPlattform.Models;
using TippPlattformMaui.Shared.Dtos;

namespace TippPlattform.Controllers.Api;
/// <summary>
/// Schnittstelle für App und Datenbank durch eine Api-Anbindung
/// </summary>
[ApiController]
[Route("api/user")]
[AllowAnonymous]
public class UserApiController : ControllerBase
{
    private readonly TippPlattformContext _context;
    public UserApiController(TippPlattformContext ctx) => _context = ctx;
    /// <summary>
    /// Ruft einen Benutzer anhand des Benutzernamens ab.
    /// </summary>
    /// <param name="username">Der Benutzername, nach dem gesucht werden soll.</param>
    /// <returns>
    /// Benutzerdaten oder 404 Not Found, wenn der Benutzer nicht existiert.
    /// </returns>
    [HttpGet("{username}")]
    public async Task<ActionResult<UserResponseDto>> GetByUsername(string username)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return NotFound();
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            BirthDate = user.Geburtstag
        };
    }
    /// <summary>
    /// Aktualisiert die Profildaten eines Benutzers.
    /// </summary>
    /// <param name="id">ID des zu aktualisierenden Benutzers</param>
    /// <param name="dto">DTO mit neuen Benutzerdaten.</param>
    /// <returns>
    /// 200 OK wenn die Aktualisierung erfolgreich war,
    /// 400 BadRequest wenn ID-Konflikt,
    /// 404 NotFound wenn der Benutzer nicht existiert.
    /// </returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Username = dto.Username;
        user.Email = dto.Email;
        if (dto.BirthDate.HasValue)
            user.Geburtstag = dto.BirthDate.Value;

        // Passwort nur ändern, wenn eins übergeben wurde
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            // TODO: Hier Hashing/Salting einbauen, falls benötigt
            user.Password = dto.Password;
        }

        await _context.SaveChangesAsync();
        return Ok("Profil erfolgreich aktualisiert");
    }
}
