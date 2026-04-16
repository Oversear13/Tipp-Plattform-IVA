using System;
using System.Collections.Generic;

namespace TippPlattform.Models;

public partial class Beitritt
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TippgruppeId { get; set; }
    public int Points { get; set; }

    public DateTime? JoinedAt { get; set; }

    public virtual Tippgruppe Tippgruppe { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
