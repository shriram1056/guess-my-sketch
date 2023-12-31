using GuessMySketch.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace GuessMySketch.Data
{
    public class AppDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to postgres with connection string from app settings
            options.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");
            modelBuilder.Entity<User>()
                     .Property(e => e.Id)
                     .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()")    // Use 
                     .IsRequired();

            modelBuilder.Entity<User>()
                .Property(e => e.Score)
                .HasDefaultValue(0);

            modelBuilder.Entity<User>()
           .Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Room>()
           .Property(e => e.GameStarted)
           .HasDefaultValue(false);

            modelBuilder.Entity<User>()
            .HasIndex(p => new { p.Name, p.RoomId }).IsUnique();
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Room> Rooms { get; set; } = null!;

    }
}