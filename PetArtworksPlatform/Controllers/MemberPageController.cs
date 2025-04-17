using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetArtworksPlatform.Models.DTOs;
using PetArtworksPlatform.Data;
using System.Threading.Tasks;
using PetArtworksPlatform.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PetArtworksPlatform.Controllers
{
    public class MemberPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemberPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin, MemberUser")]
        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentMember = await _context.Members
                .FirstOrDefaultAsync(m => m.UserId == currentUserId);
            ViewBag.CurrentMemberId = currentMember?.MemberId;

            var members = await _context.Members
                .Select(m => new MemberDTO
                {
                    MemberId = m.MemberId,
                    MemberName = m.MemberName,
                    Email = m.Email,
                    Bio = m.Bio,
                    Location = m.Location
                })
                .ToListAsync();

            return View(members);
        }

        [Authorize(Roles = "Admin, MemberUser")]
        public async Task<IActionResult> Details(int id)
        {
            var member = await _context.Members
                .Include(m => m.Followers)
                    .ThenInclude(c => c.Follower)
                .Include(m => m.Following)
                    .ThenInclude(c => c.Following)
                .Include(m => m.PetOwners)
                    .ThenInclude(po => po.Pet)
                .Where(m => m.MemberId == id)
                .Select(m => new MemberDTO
                {
                    MemberId = m.MemberId,
                    MemberName = m.MemberName,
                    Email = m.Email,
                    Bio = m.Bio,
                    Location = m.Location,
                    Followers = m.Followers.Select(c => new BasicMemberDTO
                    {
                        MemberId = c.Follower.MemberId,
                        MemberName = c.Follower.MemberName
                    }).ToList(),
                    Following = m.Following.Select(c => new BasicMemberDTO
                    {
                        MemberId = c.Following.MemberId,
                        MemberName = c.Following.MemberName
                    }).ToList(),
                    Pets = m.PetOwners.Select(po => new PetDTO
                    {
                        PetId = po.Pet.PetId,
                        Name = po.Pet.Name,
                        Type = po.Pet.Type,
                        Breed = po.Pet.Breed,
                        DOB = po.Pet.DOB,
                        HasPic = po.Pet.HasPic,
                        PetImagePath = po.Pet.HasPic ? $"/image/pet/{po.Pet.PetId}{po.Pet.PicExtension}" : null
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (member == null) return NotFound();
            return View(member);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberDTO memberDto)
        {
            if (!ModelState.IsValid) return View(memberDto);

            var member = new Member
            {
                MemberName = memberDto.MemberName,
                Email = memberDto.Email,
                Bio = memberDto.Bio,
                Location = memberDto.Location
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin, MemberUser")]
        public async Task<IActionResult> Edit(int id)
        {
            var member = await _context.Members
                .Include(m => m.MemberUser)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && member.UserId != currentUserId)
            {
                return Forbid(); 
            }

            var memberDto = new MemberDTO
            {
                MemberId = member.MemberId,
                MemberName = member.MemberName,
                Email = member.Email,
                Bio = member.Bio,
                Location = member.Location
            };

            return View(memberDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, MemberUser")]
        public async Task<IActionResult> Edit(int id, MemberDTO memberDto)
        {
            if (id != memberDto.MemberId) return BadRequest();

            if (!ModelState.IsValid) return View(memberDto);

            var member = await _context.Members
                .Include(m => m.MemberUser)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && member.UserId != currentUserId)
            {
                return Forbid(); 
            }

            member.MemberName = memberDto.MemberName;
            member.Email = memberDto.Email;
            member.Bio = memberDto.Bio;
            member.Location = memberDto.Location;

            _context.Members.Update(member);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin, MemberUser")]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.Members
                .Include(m => m.MemberUser)
                .Where(m => m.MemberId == id)
                .Select(m => new MemberDTO
                {
                    MemberId = m.MemberId,
                    MemberName = m.MemberName
                })
                .FirstOrDefaultAsync();

            if (member == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var memberWithUser = await _context.Members
                .Include(m => m.MemberUser)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (!User.IsInRole("Admin") && memberWithUser.UserId != currentUserId)
            {
                return Forbid(); 
            }

            return View(member);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, MemberUser")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members
                .Include(m => m.MemberUser)
                .Include(m => m.Followers)
                .Include(m => m.Following)
                .Include(m => m.PetOwners)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && member.UserId != currentUserId)
            {
                return Forbid(); 
            }

            var followers = _context.Connections
                .Where(c => c.FollowerId == id)
                .ToList();

            var following = _context.Connections
                .Where(c => c.FollowingId == id)
                .ToList();

            _context.Connections.RemoveRange(followers);
            _context.Connections.RemoveRange(following);

            var petOwners = _context.PetOwners
                .Where(po => po.OwnerId == id)
                .ToList();

            _context.PetOwners.RemoveRange(petOwners);

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> FollowersAndFollowing(int memberId)
        {
            var member = await _context.Members
                .Include(m => m.Followers)
                    .ThenInclude(c => c.Follower)
                .Include(m => m.Following)
                    .ThenInclude(c => c.Following)
                .FirstOrDefaultAsync(m => m.MemberId == memberId);

            if (member == null)
            {
                return NotFound();
            }

            var followers = member.Followers.Select(c => new BasicMemberDTO
            {
                MemberId = c.Follower.MemberId,
                MemberName = c.Follower.MemberName
            }).ToList();

            var following = member.Following.Select(c => new BasicMemberDTO
            {
                MemberId = c.Following.MemberId,
                MemberName = c.Following.MemberName
            }).ToList();

            var viewConnection = new FollowersAndFollowingConnection
            {
                MemberId = member.MemberId,
                MemberName = member.MemberName,
                Followers = followers,
                Following = following
            };

            return View(viewConnection);
        }
    }
}

