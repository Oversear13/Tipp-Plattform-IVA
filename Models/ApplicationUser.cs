// Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;

namespace TippPlattform.Models
{
    // Erbt von IdentityUser, was bereits eine string Id-Eigenschaft enthält
    public class ApplicationUser : IdentityUser
    {
        // Hier kannst du zusätzliche benutzerdefinierte Eigenschaften hinzufügen, die du benötigst
        // Beispiel: Wenn du die Eigenschaften aus deinem alten User.cs Modell hierher verschieben möchtest

        // Wenn du 'Username' in deinem User.cs hattest, IdentityUser hat bereits 'UserName'
        // Es ist besser, die standardmäßige 'UserName'-Eigenschaft von IdentityUser zu verwenden.

        // Beispiel für zusätzliche Properties, falls du sie brauchst:
        public DateTime? CreatedAt { get; set; } = DateTime.Now; // Setze Standardwert
        public DateTime? Geburtstag { get; set; }

        // Auch die Sammlungen, die vorher im User.cs waren, können hierher,
        // wenn sie direkt mit ApplicationUser verknüpft sind und nicht über eine Join-Tabelle wie TippgruppeMitglied abgedeckt werden
        // public virtual ICollection<Beitritt> Beitrittes { get; set; } = new List<Beitritt>();
        // public virtual ICollection<TippgruppeAdmin> TippgruppeAdmins { get; set; } = new List<TippgruppeAdmin>();
        // public virtual ICollection<Tippschein> Tippscheines { get; set; } = new List<Tippschein>();
    }
}