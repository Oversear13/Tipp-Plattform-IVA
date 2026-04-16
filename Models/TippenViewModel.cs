using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TippPlattform.Models
{
    public class TippenViewModel 
    {
        public Tippgruppe? Tippgruppe { get; set; }

		public List<Beitritt> Beitritte { get; set; } = new List<Beitritt>();

		public TippenViewModel(Tippgruppe? tippgruppe)
        {
            Tippgruppe = tippgruppe;
        }
    }
    public class TippabgabeViewModel
    {
        public Tippgruppe? TippGruppe { get; set; } // Tippgruppe Class
        public List<SpieleInTippgruppe> SpieleInGruppe { get; set; } = new List<SpieleInTippgruppe>(); // List of games in the Tippgruppe
        public List<Tippschein> Tippscheine { get; set; } = new List<Tippschein>();
        public bool IstTippGruppeAdmin { get; set; } = false;
    }
    public class VerwaltungViewModel
    {
        public Tippgruppe? Tippgruppe { get; set; }
        public bool IstMitPasswort { get; set; } = false; // Indicates if the group has a password
        public List<PunkteRegel> PunkteRegeln { get; set; } = new List<PunkteRegel>(); // Points rule for the group
        public List<SelectListItem> PunkteRegelnSelectList { get; set; } = new List<SelectListItem>();// Points rule for the dropdown menu
        public List<SpieleInTippgruppe>? SpieleInGruppe { get; set; }
        public PunkteRegel? NewPunkteRegel { get; set; } // New points rule to be added
        public List<Spiele> AlleSpiele { get; set; } = new List<Spiele>(); // List of all games
        public List<Liga> AlleLigen { get; set; } = new List<Liga>(); // List of all leagues
    }
}
