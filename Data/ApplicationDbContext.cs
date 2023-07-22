using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proiect_IR.Models;

namespace Proiect_IR.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Login> Login { get; set; }
        public DbSet<Register> Register { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Home> Home { get; set; }

    }
}
