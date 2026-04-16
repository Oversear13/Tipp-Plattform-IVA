using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace TippPlattform.Models;

public partial class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string? Username { get; set; }
    [Required]
    [StringLength(30)]
    public string? Password { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Role { get; set; }

    [Required]
    [StringLength(40)]
    public string? Email { get; set; }

    public DateTime? Geburtstag { get; set; }

    public virtual ICollection<Beitritt> Beitrittes { get; set; } = new List<Beitritt>();

    public virtual ICollection<TippgruppeAdmin> TippgruppeAdmins { get; set; } = new List<TippgruppeAdmin>();

    public virtual ICollection<Tippschein> Tippscheines { get; set; } = new List<Tippschein>();
}