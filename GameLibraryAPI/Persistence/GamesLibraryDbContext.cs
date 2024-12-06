using GameLibraryAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryAPI.Persistence
{
    public class GamesLibraryDbContext : DbContext
    {
        public DbSet<Games> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }

        public GamesLibraryDbContext(DbContextOptions<GamesLibraryDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relacionamento Games -> Developer (1:N)
            modelBuilder.Entity<Games>()
                .HasOne(g => g.Developer)
                .WithMany()
                .HasForeignKey("DeveloperId")
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento Games -> Genres (N:N)
            modelBuilder.Entity<Games>()
                .HasMany(g => g.Genres)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "GameGenres",
                    j => j.HasOne<Genre>().WithMany().HasForeignKey("GenreId"),
                    j => j.HasOne<Games>().WithMany().HasForeignKey("GameId"));

            // Relacionamento Review -> Games (1:N)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Game)
                .WithMany(g => g.Reviews)
                .HasForeignKey("GameId")
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento Review -> User (1:N)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento User -> Games (N:N)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Library)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "UserLibrary",
                    j => j.HasOne<Games>().WithMany().HasForeignKey("GameId"),
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"));
        }
    }
}
