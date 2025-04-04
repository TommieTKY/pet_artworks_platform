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
      public List<MemberDTO> OwnerList { get; set; } = new List<MemberDTO>();

      public bool HasPic { get; set; } = false;
      public string? PetImagePath { get; set; }
      public IFormFile? PetImage { get; set; }

      public List<ArtworkForOtherDto>? ListArtworks { get; set; }
      public List<ArtworkToListDto> ArtworkList { get; set; } = new List<ArtworkToListDto>();
      public List<ArtworkForOtherDto> TempArtworks { get; set; } = new List<ArtworkForOtherDto>();
      public int? NewArtworkId { get; set; }

}
