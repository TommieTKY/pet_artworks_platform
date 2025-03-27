using PetArtworksPlatform.Models.DTOs;

namespace PetArtworksPlatform.Models{

  public class PetDetails
  {
    public required PetDTO Pet { get; set; }
    public IEnumerable<PetDTO>? UserPet { get; set; }
    public List<MemberDTO> Owners { get; set; } = new List<MemberDTO>();
  }
  
}

