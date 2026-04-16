using System;
using System.Collections.Generic;

namespace TippPlattform.Models;

public partial class Spiele
{
    public int Id { get; set; }
    public int APISpielID { get; set; }
    public int TeamAId { get; set; }

    public int TeamBId { get; set; }
    // Foreign key navigation properties
    public Mannschaft TeamA { get; set; }
    public Mannschaft TeamB { get; set; }

    public int? TeamAScore { get; set; }

    public int? TeamBScore { get; set; }

    public DateTime? SpielBeginn { get; set; }

    public DateTime? SpielEnde { get; set; }

    public int? LigaId { get; set; }
    public int? Spieltag { get; set; }

    public virtual Liga? Liga { get; set; }

    public virtual ICollection<SpieleInTippgruppe> SpieleInTippgruppes { get; set; } = new List<SpieleInTippgruppe>();

    public virtual ICollection<Tippschein> Tippscheines { get; set; } = new List<Tippschein>();
}
