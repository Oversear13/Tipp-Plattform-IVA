using System;
using System.Collections.Generic;

namespace TippPlattform.Models;

public partial class Tippschein
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SpielId { get; set; }

    public int TippgruppeId { get; set; }

    public int TippA { get; set; }
    public int TippB { get; set; }
    public int Quote1 { get; set; }
    public int Quote2 { get; set; }
    public int Quote3 { get; set; }
    public int Quote4 { get; set; }
    public DateTime? PaidOut { get; set; }
    public int? Points { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Spiele Spiel { get; set; } = null!;

    public virtual Tippgruppe Tippgruppe { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
