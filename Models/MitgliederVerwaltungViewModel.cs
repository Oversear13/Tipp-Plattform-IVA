namespace TippPlattform.Models.ViewModels
{
    public class MitgliederVerwaltungViewModel
    {
        public int GruppeId { get; set; }
        public List<MitgliedEintrag> Mitglieder { get; set; }

        public class MitgliedEintrag
        {
            public int UserId { get; set; }
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public string Rolle { get; set; } = "Spieler"; // oder "Gruppenbesitzer"
            public bool IstBesitzer { get; set; } // true = darf nicht gelöscht werden
        }
    }
}
