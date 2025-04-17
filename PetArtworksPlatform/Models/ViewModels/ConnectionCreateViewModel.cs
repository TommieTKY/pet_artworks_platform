using PetArtworksPlatform.Models.DTOs;

namespace PetArtworksPlatform.Models.ViewModels
{
    public class ConnectionCreateViewModel
    {
        public bool IsAdmin { get; set; }
        public int? CurrentMemberId { get; set; }
        public List<BasicMemberDTO> AllMembers { get; set; }
        public List<BasicMemberDTO> AvailableUsers { get; set; }
    }
}
