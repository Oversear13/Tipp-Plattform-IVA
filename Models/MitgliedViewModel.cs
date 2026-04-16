public class MitgliederVerwaltungViewModel
{
    public int GruppeId { get; set; }
    public List<MitgliedViewModel> Mitglieder { get; set; } = new();
}

public class MitgliedViewModel
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime? JoinedAt { get; set; }
    public bool IsAdmin { get; set; }
}
