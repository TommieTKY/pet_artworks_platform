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

    }
}