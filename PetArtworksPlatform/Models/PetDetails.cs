using PetArtworksPlatform.Models.DTOs;

namespace PetArtworksPlatform.Models{

  public class PetDetails
  {
    public required PetDTO Pet { get; set; }
    public IEnumerable<PetDTO>? UserPet { get; set; }

    public List<MemberDTO> Owners { get; set; } = new List<MemberDTO>();
    public List<MemberDTO> OwnerList { get; set; } = new List<MemberDTO>();

    public List<ArtworkForOtherDto> ListArtworks { get; set; } = new List<ArtworkForOtherDto>();
    public List<ArtworkToListDto> ArtworkList { get; set; } = new List<ArtworkToListDto>();
    }
}
  


