using Microsoft.EntityFrameworkCore;
using SafeVault.Backend.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SafeVault.Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
