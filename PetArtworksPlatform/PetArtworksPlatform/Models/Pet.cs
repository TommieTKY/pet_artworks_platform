using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetArtworksPlatform.Models
{

  public class Pet
  {
      [Key]
      public int PetId { get; set; }

      public string? Name { get; set; }

      public string? Type { get; set; }
      public string? Breed { get; set; }

      public DateTime DOB { get; set; }

      // public int OwnerId { get; set; }

      // public Member? Owner { get; set; }

      public ICollection<PetOwner> PetOwners { get; set; } = new List<PetOwner>();

      public ICollection<Artwork>? Artworks { get; set; }

    }

    public class PetForArtworkDto
    {
        public int PetId { get; set; }
        public string Name { get; set; }
    }

}


