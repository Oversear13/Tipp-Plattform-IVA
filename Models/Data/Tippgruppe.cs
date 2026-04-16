using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TippPlattform.Models;

public partial class Tippgruppe
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Name { get; set; }
    public string? Beschreibung { get; set; }

    public int SporttypeId { get; set; }

    public DateTime? CreatedAt { get; set; }
    public string? Passwort { get; set; }
    public string? Badge { get; set; } //  Badge für die offizielle Tippgruppe

    public virtual ICollection<Beitritt> Beitrittes { get; set; } = new List<Beitritt>();

    public virtual ICollection<SpieleInTippgruppe> SpieleInTippgruppes { get; set; } = new List<SpieleInTippgruppe>();
    [ValidateNever]
    public virtual Sporttype Sporttype { get; set; } = null!;

    public virtual ICollection<TippgruppeAdmin> TippgruppeAdmins { get; set; } = new List<TippgruppeAdmin>();

    public virtual ICollection<Tippschein> Tippscheines { get; set; } = new List<Tippschein>();
    public virtual ICollection<PunkteRegel> PunkteRegeln { get; set; } = new List<PunkteRegel>();
}
