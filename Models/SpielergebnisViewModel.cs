namespace TippPlattform.Models
{
    public class SpielergebnisViewModel
    {
        public List<SpielMitUserTipp> SpielMitUserTipps { get; set; }
    }
    public class SpielMitUserTipp
    {
        public Spiele Spiel { get; set; }
        public Tippschein? UserTipp { get; set; }
    }

}
