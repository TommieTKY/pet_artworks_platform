using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetArtworksPlatform.Data;
using PetArtworksPlatform.Models;
using Microsoft.EntityFrameworkCore;
using PetArtworksPlatform.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PetArtworksPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConnectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<int?> GetCurrentMemberId()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentMember = await _context.Members
                .FirstOrDefaultAsync(m => m.UserId == currentUserId);
            return currentMember?.MemberId;
        }

        /// <summary>
        /// Creates a new follow relationship between two members.
        /// </summary>
        /// <example>
        /// POST api/Connection/Follow/1/2 -> "Followed successfully"
        /// ------
        /// Error: Already following
        /// POST api/Connection/Follow/1/2 -> "You are already following this user."
        /// </example>
        /// <returns>
        /// Success message if follow is successful. Error message if already following.
        /// </returns>
        [HttpPost("NewFollow/{memberId}/{followingId}")]
        [Authorize(Roles = "Admin, MemberUser")]
        public async Task<ActionResult> FollowUser(int memberId, int followingId)
        {
            var currentMemberId = await GetCurrentMemberId();
            if (currentMemberId == null)
            {
                return Unauthorized("User not logged in.");
            }

            if (!User.IsInRole("Admin") && currentMemberId != memberId)
            {
                return Forbid("You can only perform actions on your own account.");
            }

            if (memberId == followingId)
            {
                return BadRequest("You cannot follow yourself.");
            }

            var existingConnection = await _context.Connections
                .FirstOrDefaultAsync(c => c.FollowerId == memberId && c.FollowingId == followingId);

            if (existingConnection != null)
            {
                return Conflict("You are already following this user.");
            }

            var newConnection = new Connection
            {
                FollowerId = memberId,
                FollowingId = followingId
            };

            _context.Connections.Add(newConnection);
            await _context.SaveChangesAsync();

            return Ok("Followed successfully");
        }

        /// <summary>
        /// Deletes a existing connection between two members.
        /// </summary>
        /// <example>
        /// POST api/Connection/Unfollow/1/2 -> "Unfollowed successfully"
        /// ------
        /// Error: Connection not found
        /// POST api/Connection/Unfollow/1/2 -> "Connection not found."
        /// </example>
        /// <returns>
        /// Success message if follow is successful. Error message if already following.
        /// </returns>
        [HttpDelete("Unfollow/{memberId}/{followingId}")]
        [Authorize(Roles = "Admin, MemberUser")]
        public async Task<IActionResult> UnfollowUser(int memberId, int followingId)
        {
            var currentMemberId = await GetCurrentMemberId();
            if (currentMemberId == null)
            {
                return Unauthorized("User not logged in.");
            }

            if (!User.IsInRole("Admin") && currentMemberId != memberId)
            {
                return Forbid("You can only perform actions on your own account.");
            }

            var connection = await _context.Connections
                .FirstOrDefaultAsync(c => c.FollowerId == memberId && c.FollowingId == followingId);

            if (connection == null)
            {
                return NotFound("Connection not found.");
            }

            _context.Connections.Remove(connection);
            await _context.SaveChangesAsync();

            return Ok("Unfollowed successfully");
        }
    }
}