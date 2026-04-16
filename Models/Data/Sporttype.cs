using System;
using System.Collections.Generic;

namespace TippPlattform.Models;

public partial class Sporttype
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Tippgruppe> Tippgruppens { get; set; } = new List<Tippgruppe>();
}
