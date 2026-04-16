using Microsoft.EntityFrameworkCore;
using TippPlattform.Models;

namespace TippPlattform.Data
{
    public class TippPlattformContext2 : DbContext
    {
        public TippPlattformContext2(DbContextOptions<TippPlattformContext2> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }


    }
}
