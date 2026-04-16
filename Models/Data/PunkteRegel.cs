namespace TippPlattform.Models
{
    public class PunkteRegel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Beschreibung { get; set; }
        public int Quote1 { get; set; }
        public int Quote2 { get; set; }
        public int Quote3 { get; set; }
        public int Quote4 { get; set; }
        public int Tippgruppe_Id { get; set; }
        public virtual Tippgruppe Tippgruppe { get; set; } = null!;
        public ICollection<SpieleInTippgruppe> SpieleInTippgruppe { get; set; }
    }
}
