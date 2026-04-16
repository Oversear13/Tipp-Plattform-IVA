using TippPlattform.Models;
using static TippPlattform.Controllers.AdminController;

namespace TippPlattform.Services
{
    /// <summary>
    /// Service for interacting with the Football Data API to retrieve match data.
    /// </summary>
    public class FootballDataService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.football-data.org/v4/";
        private readonly string _apiKey;

        public FootballDataService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["FootballData:ApiKey"];

            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", _apiKey);
        }
        public async Task<string> GetScheduledMatchesAsync()
        {
            string url = $"{BaseUrl}matches?status=SCHEDULED";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// Retrieves matches based on the specified mode and date range.
        /// </summary>
        /// <param name="model">
        /// Date and mode for retrieving matches.
        /// mode(InsertNew or UpdateExisting) determines the status (Scheduled or Finished) of the matches.
        /// </param>
        /// <returns>Json Spiel Daten</returns>
        public async Task<string> GetMatchesAsync(SaveMatchesToDatabaseVM model)
        {
            string status = "";
            string from = "";
            string to = "";
            switch (model.Mode)
            {
                case MatchSaveMode.InsertNew:
                    status = "SCHEDULED";
                    var fromDate = model.Date ?? DateTime.Now;
                    from = fromDate.ToString("yyyy-MM-dd");
                    to = fromDate.AddDays(9).ToString("yyyy-MM-dd");
                    break;
                case MatchSaveMode.UpdateExisting:
                    status = "FINISHED";
                    var fromDate2 = model.Date ?? DateTime.Now.AddDays(-9);
                    from = fromDate2.ToString("yyyy-MM-dd");
                    to = fromDate2.AddDays(9).ToString("yyyy-MM-dd");
                    break;
            }

            string url = $"{BaseUrl}matches?competitions=2021,2002,2019,2014&status={status}&dateFrom={from}&dateTo={to}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

    }
}
