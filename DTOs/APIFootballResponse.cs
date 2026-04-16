using System.Text.Json.Serialization;

namespace TippPlattform.DTOs
{
    public class APIFootballResponse
    {
        [JsonPropertyName("matches")]
        public List<ApiMatch> Matches { get; set; }
    }
    public class ApiMatch
    {
        [JsonPropertyName("id")]
        public int MatchID { get; set; }
        [JsonPropertyName("competition")]
        public ApiLeague League { get; set; }
        [JsonPropertyName("utcDate")]
        public DateTime UtcDate { get; set; }
        [JsonPropertyName("matchday")]
        public int Matchday { get; set; }
        [JsonPropertyName("homeTeam")]
        public ApiTeam HomeTeam { get; set; }
        [JsonPropertyName("awayTeam")]
        public ApiTeam AwayTeam { get; set; }
        [JsonPropertyName("score")]
        public ApiScore Scores { get; set; }
    }
    public class ApiLeague
    {
        [JsonPropertyName("id")]
        public int LeagueId { get; set; }
        [JsonPropertyName("name")]
        public string LeagueName { get; set; }
    }
    public class ApiTeam
    {
        [JsonPropertyName ("id")]
        public int TeamId { get; set; }
        [JsonPropertyName("name")]
        public string TeamName { get; set; }
    }
    public class ApiScore
    {
        [JsonPropertyName("winner")]
        public string Winner { get; set; }
        [JsonPropertyName("fullTime")]
        public ApiScoreFulltime ApiScoreFulltime { get; set; }
    }
    public class ApiScoreFulltime 
    {
        [JsonPropertyName("home")]
        public int? HomeScore { get; set; }
        [JsonPropertyName("away")]
        public int? AwayScore { get; set; }
    }

}
