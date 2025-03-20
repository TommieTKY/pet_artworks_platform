using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PetArtworksPlatform.Models
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? UserId { get; set; } 
        public IdentityUser? User { get; set; } 

        // Many to Many Relationship: Followers and Following
        public ICollection<Connection> Followers { get; set; } = new List<Connection>();
        public ICollection<Connection> Following { get; set; } = new List<Connection>();

        // Many to Many Relationship: Pet Owners
        public ICollection<PetOwner> PetOwners { get; set; } = new List<PetOwner>();
    }
}