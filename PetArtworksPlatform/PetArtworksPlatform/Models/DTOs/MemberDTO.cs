using System;

namespace PetArtworksPlatform.Models.DTOs
{
    public class MemberDTO
    {
        public int MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public int OwnerId => MemberId;

        public List<BasicMemberDTO> Followers { get; set; } = new List<BasicMemberDTO>();
        public List<BasicMemberDTO> Following { get; set; } = new List<BasicMemberDTO>();
        public List<PetDTO> Pets { get; set; } = new List<PetDTO>();

    }
}