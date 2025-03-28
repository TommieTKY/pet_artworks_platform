using System;

namespace PetArtworksPlatform.Models.DTOs
{
    public class ConnectionDTO
    {
        public int ConnectionId { get; set; }
        public int FollowerId { get; set; }
        public int FollowingId { get; set; }
        public string FollowerName { get; set; } = string.Empty; 
        public string FollowingName { get; set; } = string.Empty; 
    }
}