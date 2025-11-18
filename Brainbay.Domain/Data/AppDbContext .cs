using Brainbay.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Brainbay.Domain.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
        public DbSet<Character> Characters => Set<Character>();
        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Character>().HasIndex(c => c.ExternalId).IsUnique();
            b.Entity<Character>().Property(c => c.Name).HasMaxLength(256);
            b.Entity<Character>().Property(c => c.Origin).HasMaxLength(256);
            b.Entity<Character>().Property(c => c.Location).HasMaxLength(256);
        }
    }
}
