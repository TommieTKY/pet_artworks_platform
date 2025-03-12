using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using pawpals.Models;

namespace pawpals.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Member> Members { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Connection> Connections { get; set; }
    public DbSet<PetOwner> PetOwners { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Member-Pet table relationships OLD - one to many 
        // modelBuilder.Entity<Pet>()
        // .HasOne(p => p.Owner) 
        // .WithMany() 
        // .HasForeignKey(p => p.OwnerId) 
        // .OnDelete(DeleteBehavior.Restrict);

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

        modelBuilder.Entity<Connection>()
            .HasOne(c => c.Following)
            .WithMany(u => u.Following)
            .HasForeignKey(c => c.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}