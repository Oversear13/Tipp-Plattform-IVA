namespace TippPlattform.Models
{
    public class AdminViewModel : SaveMatchesToDatabaseVM
    {
        public List<Spiele> BevorstehendeSpiele { get; set; }
        public List<Spiele> AusstehendeSpielergebnisse { get; set; }
        public List<Spiele> Spielergebnisse { get; set; }
    }
    public class SaveMatchesToDatabaseVM
    {
        public MatchSaveMode Mode { get; set; }
        public DateTime? Date { get; set; }
    }
    public enum MatchSaveMode
    {
        InsertNew,
        UpdateExisting
    }
    public class SpileListeViewModel
    {
        public List<Liga> Ligen { get; set; }
    }
    public class TippgruppenVerwaltung
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Beschreibung { get; set; }
        public string SportArt { get; set; }
        public string Gruppenbesitzer { get; set; }
        public int AnzalMitglieder { get; set; }
        public string? Badge { get; set; } // Badge for the official Tippgruppe
    }
}
