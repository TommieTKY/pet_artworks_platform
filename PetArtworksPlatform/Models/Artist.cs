using System.ComponentModel.DataAnnotations;
using PetArtworksPlatform.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PetArtworksPlatform.Models
{
    public class Artist
    {
        [Key]
        public int ArtistID { get; set; } 
        public string ArtistName { get; set; }
        public string ArtistBiography { get; set; }

        public ICollection<Artwork>? Artworks { get; set; }

        public IdentityUser? ArtistUser { get; set; }
    }

    public class ArtistToListDto
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public string ArtistBiography { get; set; }
        public int ArtworkCount { get; set; } 
    }

    public class ArtistPersonDto
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public string ArtistBiography { get; set; }
        public List<ArtworkForOtherDto>? ListArtworks { get; set; }
        public IdentityUser? ArtistUser { get; set; }
    }

    public class ArtistForOtherDto
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        //public IdentityUser? ArtistUser { get; set; }
    }
}
