using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetArtworksPlatform.Models
{
    public class PetOwner
    {
        [Key]
        public int PetOwnerId { get; set; }

        public int PetId { get; set; }
        public int OwnerId { get; set; }

        [ForeignKey("PetId")]
        public Pet? Pet { get; set; }

        [ForeignKey("OwnerId")]
        public Member? Owner { get; set; }
    }
}