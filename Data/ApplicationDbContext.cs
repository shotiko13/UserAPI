using Hw4.Models;
using Microsoft.EntityFrameworkCore;

namespace Hw4.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }

        public DbSet<ApplicationUser> Users { get; set; }
    }
}
