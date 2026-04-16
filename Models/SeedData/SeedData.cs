namespace TippPlattform.Models.SeedData;

using TippPlattform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;


public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new TippPlattformContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<TippPlattformContext>>()))
        {
            // Look for any users.
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Username = "Hura",
                        Password = "50Pilze",
                        Role = "Admin",
                        Email = "Hura.Pilze@gmail.com",

                    },
                    new User
                    {
                        Username = "Heyo",
                        Password = "49Bäume",
                        Role = "User",
                        Email = "Heyo.49Bäume@gmail.com",
                    },
                    new User
                    {
                        Username = "Haya",
                        Password = "10509546",
                        Role = "Guest",
                        Email = "Maxmustermann@gmail.com",
                    },
                    new User
                    {
                        Username = "Guest",
                        Password = "Katzensindcool",
                        Role = "Guest",
                        Email = "tfdik@gmail.com",
                    }
                );
            }
            context.SaveChanges();
            if (!context.Sporttypen.Any())
            {
                context.Sporttypen.AddRange(
                    new Sporttype
                    {
                        Name = "Fußball"
                    },
                    new Sporttype
                    {
                        Name = "Basketball"
                    }
                );
            }
            if (!context.Ligen.Any())
            {
                context.Ligen.AddRange(
                    new Liga
                    {
                        ApiLigaId = 2002,
                        LigaName = "Bundesliga"
                    },
                    new Liga
                    {
                        ApiLigaId = 2017,
                        LigaName = "La Liga (Spanien)"
                    },
                    new Liga
                    {
                        ApiLigaId = 2019,
                        LigaName = "Serie A (Italien)"
                    },
                    new Liga
                    {
                        ApiLigaId = 2021,
                        LigaName = "Premier League"
                    }
                );
            }
            context.SaveChanges();
        }
    }
}

