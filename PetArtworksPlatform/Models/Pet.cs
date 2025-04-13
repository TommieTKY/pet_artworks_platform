using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetArtworksPlatform.Models
{

    public class Pet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PetId { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Breed { get; set; }
        public DateTime DOB { get; set; }
        public ICollection<PetOwner> PetOwners { get; set; } = new List<PetOwner>();
        public bool HasPic { get; set; } = false;
        public string? PicExtension { get; set; }
        public ICollection<Artwork>? Artworks { get; set; }
    }

    public class PetForArtworkDto
    {
        public int PetId { get; set; }
        public string Name { get; set; }
    }

}

