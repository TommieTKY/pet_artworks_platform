using System;

namespace PetArtworksPlatform.Models.DTOs;

public class PetDTO
  {
    public int PetId { get; set; }
    public string Name { get; set; } = string.Empty; 
    public string Type { get; set; } = string.Empty; 
    public string Breed { get; set; } = string.Empty; 
    public DateTime DOB { get; set; }
    public List<int> OwnerIds { get; set; } = new List<int>();
    public List<ArtworkForOtherDto>? ListArtworks { get; set; }
}
