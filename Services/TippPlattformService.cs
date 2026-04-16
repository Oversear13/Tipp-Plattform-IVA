using Microsoft.EntityFrameworkCore;
using TippPlattform.Models;

namespace TippPlattform.Services
{
    public class TippPlattformService
    {
        private readonly TippPlattformContext _tippPlattformContext;

        public TippPlattformService(TippPlattformContext tippPlattformContext)
        {
            _tippPlattformContext = tippPlattformContext;
        }
        // Method to calculate points return amount of points
        public int CalculatePoints(Tippschein? tippschein)
        {
            if(tippschein == null)
            {
                return 0;
            }
            int points = 0;

            var getQuote = _tippPlattformContext.SpieleInTippgruppen
                .Include(x => x.PunkteRegel)
                .FirstOrDefault(x => x.SpielId == tippschein.SpielId)?
                .PunkteRegel;

            int quote1 = getQuote?.Quote1 ?? 4;
            int quote2 = getQuote?.Quote2 ?? 3;
            int quote3 = getQuote?.Quote3 ?? 2;
            int quote4 = getQuote?.Quote4 ?? 0;

            if (tippschein.TippA == tippschein.Spiel.TeamAScore && tippschein.TippB == tippschein.Spiel.TeamBScore)
            {
                points = quote1;  // For exact match
            }
            else if ((tippschein.TippA - tippschein.TippB) == (tippschein.Spiel.TeamAScore ?? 0) - (tippschein.Spiel.TeamBScore ?? 0))
            {
                points = quote2;  // For goal difference
            }
            else if ((tippschein.TippA > tippschein.TippB && tippschein.Spiel.TeamAScore > tippschein.Spiel.TeamBScore) ||
                     (tippschein.TippA < tippschein.TippB && tippschein.Spiel.TeamAScore < tippschein.Spiel.TeamBScore) ||
                     (tippschein.TippA == tippschein.TippB && tippschein.Spiel.TeamAScore == tippschein.Spiel.TeamBScore))
            {
                points = quote3;  // For predicting winner correctly (score doesn't need to match exactly)
            }
            else
            {
                points = quote4;  // No points
            }

            return points;
        }
    }
}
