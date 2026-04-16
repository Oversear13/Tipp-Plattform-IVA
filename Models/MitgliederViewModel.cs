using System.Collections.Generic;

namespace TippPlattform.Models
{
    public class MitgliederViewModel
    {
        public int TippgruppeId { get; set; }
        public string GruppenName { get; set; } = string.Empty;
        public bool IstAdmin { get; set; } // Ist der aktuelle Benutzer Admin der Gruppe?

        // Enthält die Liste der TippgruppeMitglied-Objekte
        public List<TippgruppeMitglied> Mitglieder { get; set; } = new();
    }
}