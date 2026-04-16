namespace TippPlattform.Models
{
    public class Nachricht
    {
        public int Id { get; set; }

        public string Nachrichtentext { get; set; } = string.Empty;

        public int SenderId { get; set; }
        public int EmpfaengerId { get; set; }

        public DateTime SendDatum { get; set; } = DateTime.UtcNow;

        public DateTime? GelesenDatum { get; set; }
        public int EingeladeneGruppeId { get; set; }

        // Optional (wenn du Users hast)
        public virtual User Sender { get; set; }
        public virtual User Empfaenger { get; set; }
    }
}
