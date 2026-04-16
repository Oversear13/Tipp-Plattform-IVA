using System;
using System.Collections.Generic;

namespace TippPlattform.Models;

public partial class Mannschaft
{
    public int Id { get; set; }
    public int APIMannschaftID { get; set; }
    public string? Name { get; set; }

    public int? Rang { get; set; }

    public int? LigaId { get; set; }

    public virtual Liga? Liga { get; set; }

    public virtual ICollection<Spiele> HomeMatches { get; set; }
    public virtual ICollection<Spiele> AwayMatches { get; set; }
}
