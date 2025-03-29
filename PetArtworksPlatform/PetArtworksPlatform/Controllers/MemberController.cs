using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pawpals.Data;
using pawpals.Models;
using Microsoft.EntityFrameworkCore;
using pawpals.Models.DTOs;

namespace pawpals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MemberController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        // Gets a specific member by ID
        /// </summary>
        /// <param name="id">The ID of the member</param>
        /// <example>
        /// GET api/Member/FindMember/1
        /// </example>
        /// <returns>
        /// The member details if found, otherwise a 404 response.
        /// </returns>
        /// <response code="200">Returns the member's information</response>
        /// <response code="404">If no member is found with the given ID</response>
        [HttpGet("FindMember/{id}")]
        public async Task<ActionResult<MemberDTO>> GetMember(int id)
        {
            var member = await _context.Members
                .Include(m => m.Followers)
                .Include(m => m.Following)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null)
            {
                return NotFound();
            }

            var memberDto = new MemberDTO
            {
                MemberId = member.MemberId,
                MemberName = member.MemberName,
                Email = member.Email,
                Bio = member.Bio,
                Location = member.Location
            };

            return memberDto;
        }

        /// <summary>
        /// Show a list of members who follow the specified member.
        /// </summary>
        /// <param name="memberId">The ID of the member whose followers should be retrieved</param>
        /// <example>
        /// GET api/Member/Followers/1
        /// </example>
        /// <returns>
        /// A list of basic member details of the followers.
        /// </returns>
        /// <response code="200">Returns a list of followers</response>
        /// <response code="404">If the member with the given ID is not found</response>
        [HttpGet("/api/Member/Followers/{memberId}")]
        public async Task<ActionResult<List<BasicMemberDTO>>> GetFollowers(int memberId)
        {
            var followers = await _context.Connections
                .Where(c => c.FollowingId == memberId)
                .Include(c => c.Follower)
                .Select(c => new BasicMemberDTO
                {
                    MemberId = c.Follower.MemberId,
                    MemberName = c.Follower.MemberName
                })
                .ToListAsync();

            return Ok(followers);
        }

        /// <summary>
        /// Show a list of members that the specified member is following.
        /// </summary>
        /// <param name="memberId">The ID of the member whose followings should be retrieved</param>
        /// <example>
        /// GET api/Member/Following/1
        /// </example>
        /// <returns>
        /// A list of members that the given member follows.
        /// </returns>
        /// <response code="200">Returns a list of followings</response>
        /// <response code="404">If the member with the given ID is not found</response>
        [HttpGet("/api/Member/Following/{memberId}")]
        public async Task<ActionResult<List<BasicMemberDTO>>> GetFollowing(int memberId)
        {
            var following = await _context.Connections
                .Where(c => c.FollowerId == memberId)
                .Include(c => c.Following)
                .Select(c => new BasicMemberDTO
                {
                    MemberId = c.Following.MemberId,
                    MemberName = c.Following.MemberName
                })
                .ToListAsync();

            return Ok(following);
        }

        /// <summary>
        /// Retrieves a list of all members in the system.
        /// </summary>
        /// <example>
        /// GET api/Member/ListMembers
        /// </example>
        /// <returns>
        /// A list of all registered members.
        /// </returns>
        /// <response code="200">Returns a list of members</response>
        [HttpGet("ListMembers")]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetMembers()
        {
            var members = await _context.Members
                .Include(m => m.Followers)
                .Include(m => m.Following)
                .Select(m => new MemberDTO
                {
                    MemberId = m.MemberId,
                    MemberName = m.MemberName,
                    Email = m.Email,
                    Bio = m.Bio,
                    Location = m.Location
                })
                .ToListAsync();

            return members;
        }

        /// <summary>
        /// Adds a new member to the system.
        /// </summary>
        /// <param name="memberDto">The member data transfer object containing details of the new member</param>
        /// <example>
        /// POST api/Member/AddMember
        /// {
        ///   "memberName": "John Doe",
        ///   "email": "john@example.com",
        ///   "bio": "Hi！",
        ///   "location": "New York"
        /// }
        /// </example>
        /// <returns>
        /// The newly created member with their details.
        /// </returns>
        /// <response code="201">Returns the newly created member</response>
        /// <response code="400">If the input data is invalid</response>
        [HttpPost("AddMember")]
        public async Task<ActionResult<MemberDTO>> PostMember(MemberDTO memberDto)
        {
            var newMember = new Member
            {
                MemberName = memberDto.MemberName,
                Email = memberDto.Email,
                Bio = memberDto.Bio,
                Location = memberDto.Location
            };

            _context.Members.Add(newMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMember), new { id = newMember.MemberId }, memberDto);
        }

        /// <summary>
        /// Updates the details of an existing member.
        /// </summary>
        /// <param name="id">The ID of the member to update</param>
        /// <param name="memberDto">The updated member details</param>
        /// <example>
        /// PUT api/Member/UpdateMember/1
        /// {
        ///   "memberId": 1,
        ///   "memberName": "John Doe Updated",
        ///   "email": "john@etest.com",
        ///   "bio": "Loves dogs",
        ///   "location": "Toronto"
        /// }
        /// </example>
        /// <returns>
        /// No content if the update is successful.
        /// </returns>
        /// <response code="204">Update successful</response>
        /// <response code="400">If the ID in the URL does not match the ID in the request body</response>
        /// <response code="404">If the member with the given ID is not found</response>
        [HttpPut("UpdateMember/{id}")]
        public async Task<IActionResult> PutMember(int id, MemberDTO memberDto)
        {
            if (id != memberDto.MemberId)
            {
                return BadRequest("Member ID in the URL does not match the ID in the body.");
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            member.MemberName = memberDto.MemberName;
            member.Email = memberDto.Email;
            member.Bio = memberDto.Bio;
            member.Location = memberDto.Location;

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.MemberId == id);
        }

        /// <summary>
        /// Deletes a member from the system.
        /// </summary>
        /// <param name="id">The ID of the member to delete</param>
        /// <example>
        /// DELETE api/Member/DeleteMember/1
        /// </example>
        /// <returns>
        /// No content if the deletion is successful.
        /// </returns>
        /// <response code="204">Operation successful</response>
        /// <response code="404">If the member with the given ID is not found</response>
        [HttpDelete("DeleteMember/{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}