using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TippPlattform.Models
{
    public class TippGruppenViewModel
    {
        public List<TippGruppe> TippGruppen { get; set; } = new List<TippGruppe>();
    }

    public class TippGruppe : Tippgruppe
    {
        public string SportArt { get; set; }
        public int AnzahlSpiele { get; set; }
        public DateTime? Tipptermin { get; set; } // Der nächste kommende Termin 
        public bool IstAdmin { get; set; } = false; // Gibt an, ob der aktuelle User Admin der Gruppe ist
    }

    public class CreateTippGruppeViewModel
    {
        [Required]
        public string Name { get; set; }

        public string? Beschreibung { get; set; }

        [Display(Name = "Sportart")]
        public int SporttypeId { get; set; }  //  WICHTIG: korrekt für EF und View-Binding

        public string? Passwort { get; set; }
        public string? Badge { get; set; } //  Badge für die offizielle Tippgruppe

        //  Dropdown-Daten für View
        public List<SelectListItem>? VerfügbareSportarten { get; set; }
    }
}