using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace pawpals.Models
{

  public class Connection
  {
      public int ConnectionId { get; set; }
      public int FollowerId { get; set; } 
      public int FollowingId { get; set; } 
      public Member? Follower { get; set; } 
      public Member? Following { get; set; } 
        
  }

}


