using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TippPlattform.Models
{
    public class TippgruppeMitglied
    {
        [Key, Column(Order = 0)]
        public int TippgruppeId { get; set; }

        [Key, Column(Order = 1)]
        // Der UserId-Typ muss string sein, da ApplicationUser.Id ein string ist
        public string UserId { get; set; }

        public string Rolle { get; set; } = "Mitglied";

        public DateTime JoinedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual Tippgruppe Tippgruppe { get; set; }
        public virtual ApplicationUser User { get; set; } // Muss ApplicationUser sein
    }
}