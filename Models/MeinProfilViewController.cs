using System;
using System.ComponentModel.DataAnnotations;

namespace TippPlattform.Models
{
    public class MeinProfilViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Geburtstag { get; set; }

        // Optional: Nur anzeigen, wenn du das aktuelle Passwort brauchst
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        public string? NeuesPasswort { get; set; }

        [DataType(DataType.Password)]
        [Compare("NeuesPasswort", ErrorMessage = "Die Passwörter stimmen nicht überein.")]
        public string? PasswortBestaetigung { get; set; }
    }
}
