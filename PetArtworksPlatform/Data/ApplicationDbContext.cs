using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetArtworksPlatform.Models;

namespace PetArtworksPlatform.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Member> Members { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Connection> Connections { get; set; }
    public DbSet<PetOwner> PetOwners { get; set; }
    public DbSet<Artwork> Artworks { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Exhibition> Exhibitions { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PetOwner>()
            .HasKey(po => new { po.PetId, po.OwnerId }); 

        modelBuilder.Entity<PetOwner>()
            .HasOne(po => po.Pet) 
            .WithMany(p => p.PetOwners) 
            .HasForeignKey(po => po.PetId);

        modelBuilder.Entity<PetOwner>()
        .HasOne(po => po.Owner)
        .WithMany(m => m.PetOwners)
        .HasForeignKey(po => po.OwnerId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Connection>()
            .HasKey(c => c.ConnectionId);

        modelBuilder.Entity<Connection>()
            .HasOne(c => c.Follower)
            .WithMany(m => m.Following)
            .HasForeignKey(c => c.FollowerId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Connection>()
            .HasOne(c => c.Following)
            .WithMany(m => m.Followers)
            .HasForeignKey(c => c.FollowingId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Connection>()
            .HasIndex(c => new { c.FollowerId, c.FollowingId })
            .IsUnique();
    }
    
}