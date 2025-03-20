using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetArtworksPlatform.Models;

namespace PetArtworksPlatform.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Exhibition> Exhibitions { get; set; }

        public DbSet<Connection> Connections { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<PetOwner> PetOwners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Member-Pet table relationships NEW - many to many
            modelBuilder.Entity<PetOwner>()
            .HasKey(po => po.PetOwnerId);

            modelBuilder.Entity<PetOwner>()
                .HasOne(po => po.Pet) // PetOwner - one Pet
                .WithMany(p => p.PetOwners) // Pet - many PetOwners
                .HasForeignKey(po => po.PetId); // foreignKey PetId

            modelBuilder.Entity<PetOwner>()
                .HasOne(po => po.Owner) // PetOwner - Owner
                .WithMany(m => m.PetOwners) // Member - many PetOwners
                .HasForeignKey(po => po.OwnerId); // foreignKey OwnerId

            // Member-Connection table relationships - Bridge table
            modelBuilder.Entity<Connection>()
                .HasOne(c => c.Follower)
                .WithMany(u => u.Followers)
                .HasForeignKey(c => c.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
