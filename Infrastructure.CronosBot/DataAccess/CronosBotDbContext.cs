using Domain.CronosBot.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CronosBot.DataAccess
{
    internal class CronosBotDbContext : DbContext
    {
        public CronosBotDbContext(DbContextOptions options): base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("cronosbot");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ChatSession> Sessions { get; set; }

    }
}
