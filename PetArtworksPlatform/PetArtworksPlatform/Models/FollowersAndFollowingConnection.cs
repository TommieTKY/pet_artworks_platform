using System;
using pawpals.Models.DTOs;

namespace pawpals.Models
{
    public class FollowersAndFollowingConnection
    {
        public int MemberId { get; set; }
        public string? MemberName { get; set; }
        public required List<BasicMemberDTO> Followers { get; set; }
        public required List<BasicMemberDTO> Following { get; set; }
    }
}