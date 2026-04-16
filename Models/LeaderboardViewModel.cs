namespace TippPlattform.Models
{
    public class LeaderboardViewModel
    {
        public Tippgruppe Tippgruppe { get; set; }
        public List<ScoreboardEntry> Leaderboard { get; set; }
        public List<LastGamesEntry> LastGames { get; set; }
    }
    public class ScoreboardEntry
    {
        public string Username { get; set; }
        public int Points { get; set; }
        public List<LastTippEntry> LastTipp { get; set; }
    }
    public class LastTippEntry
    {
        public int SpielId { get; set; }
        public string UserTipp { get; set; }
        public int UserGewinn { get; set; }
    }
    public class LastGamesEntry
    {
        public int SpielId { get; set; }
        public string TeamA { get; set; }
        public int TeamAScore { get; set; }
        public string TeamB { get; set; }
        public int TeamBScore { get; set; }
    }
}
