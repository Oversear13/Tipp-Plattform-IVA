using System;
using System.Collections.Generic;

namespace TippPlattform.Models;

public partial class SpieleInTippgruppe
{
    public int Id { get; set; }

    public int SpielId { get; set; }

    public int TippgruppeId { get; set; }
    public int? PunkteRegelId { get; set; }

    public virtual Spiele Spiel { get; set; } = null!;

    public virtual Tippgruppe Tippgruppe { get; set; } = null!;
    public virtual PunkteRegel? PunkteRegel { get; set; }
}
