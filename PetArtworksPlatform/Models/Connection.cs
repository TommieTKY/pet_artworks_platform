using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetArtworksPlatform.Models
{

  public class Connection
  {
      public int ConnectionId { get; set; }
      public int FollowerId { get; set; } 
      public int FollowingId { get; set; } 

      [ForeignKey("FollowerId")]
      public Member? Follower { get; set; } 
      
      [ForeignKey("FollowingId")]
      public Member? Following { get; set; } 
        
  }

}


