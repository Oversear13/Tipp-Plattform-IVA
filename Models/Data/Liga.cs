using System;
using System.Collections.Generic;

namespace TippPlattform.Models;

public partial class Liga
{
    public int Id { get; set; }
    public int ApiLigaId { get; set; }
    public string? LigaName { get; set; }
    public virtual ICollection<Mannschaft> Mannschafts { get; set; } = new List<Mannschaft>();

    public virtual ICollection<Spiele> Spieles { get; set; } = new List<Spiele>();
}
