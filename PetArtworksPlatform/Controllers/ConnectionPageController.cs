using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetArtworksPlatform.Data;
using PetArtworksPlatform.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using PetArtworksPlatform.Models.ViewModels;
using PetArtworksPlatform.Models.DTOs;

namespace PetArtworksPlatform.Controllers
{
    [Authorize(Roles = "Admin, MemberUser")]
    public class ConnectionPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConnectionPageController(ApplicationDbContext context)
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

        public async Task<IActionResult> Index()
        {
            var currentMemberId = await GetCurrentMemberId();
            if (!User.IsInRole("Admin") && currentMemberId == null)
            {
                TempData["Error"] = "You must create a member profile before accessing connections.";
                return RedirectToAction("Create", "MemberPage");
            }

            var query = _context.Connections
                .Include(c => c.Follower)
                .Include(c => c.Following)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(c => c.FollowerId == currentMemberId);
            }

            var connections = await query.ToListAsync();
            return View(connections);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            bool isAdmin = User.IsInRole("Admin");
            var currentMemberId = await GetCurrentMemberId();

            if (!isAdmin && currentMemberId == null)
            {
                TempData["Error"] = "You must create a member profile before creating connections.";
                return RedirectToAction("Create", "MemberPage");
            }

            var viewModel = new ConnectionCreateViewModel
            {
                IsAdmin = isAdmin,
                CurrentMemberId = currentMemberId
            };

            if (isAdmin)
            {
                var allMembers = await _context.Members
                    .Select(m => new BasicMemberDTO
                    {
                        MemberId = m.MemberId,
                        MemberName = m.MemberName
                    })
                    .ToListAsync();

                viewModel.AllMembers = allMembers;

                if (currentMemberId != null)
                {
                    var followingIds = await _context.Connections
                        .Where(c => c.FollowerId == currentMemberId)
                        .Select(c => c.FollowingId)
                        .ToListAsync();

                    var availableUsers = allMembers
                        .Where(m => m.MemberId != currentMemberId && !followingIds.Contains(m.MemberId))
                        .ToList();

                    viewModel.AvailableUsers = availableUsers;
                }
                else
                {
                    viewModel.AvailableUsers = allMembers;
                }
            }
            else
            {
                var followingIds = await _context.Connections
                    .Where(c => c.FollowerId == currentMemberId)
                    .Select(c => c.FollowingId)
                    .ToListAsync();

                var availableUsers = await _context.Members
                    .Where(m => m.MemberId != currentMemberId && !followingIds.Contains(m.MemberId))
                    .Select(m => new BasicMemberDTO
                    {
                        MemberId = m.MemberId,
                        MemberName = m.MemberName
                    })
                    .ToListAsync();

                viewModel.AvailableUsers = availableUsers;
            }

            return View("Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int followingId, int? followerId = null)
        {
            try
            {
                bool isAdmin = User.IsInRole("Admin");
                var currentMemberId = await GetCurrentMemberId();

                if (!isAdmin && currentMemberId == null)
                {
                    TempData["Error"] = "You must create a member profile before creating connections.";
                    return RedirectToAction("Create", "MemberPage");
                }

                int actualFollowerId = isAdmin && followerId.HasValue
                    ? followerId.Value
                    : currentMemberId.Value;

                if (actualFollowerId == followingId)
                {
                    ModelState.AddModelError(string.Empty, "You cannot follow yourself.");
                    return await LoadAvailableUsersAndReturnView(isAdmin);
                }

                var targetUser = await _context.Members.FindAsync(followingId);
                if (targetUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Selected user does not exist.");
                    return await LoadAvailableUsersAndReturnView(isAdmin);
                }

                var existing = await _context.Connections
                    .FirstOrDefaultAsync(c => c.FollowerId == actualFollowerId && c.FollowingId == followingId);
                if (existing != null)
                {
                    ModelState.AddModelError(string.Empty, "You are already following this user.");
                    return await LoadAvailableUsersAndReturnView(isAdmin);
                }

                var connection = new Connection
                {
                    FollowerId = actualFollowerId,
                    FollowingId = followingId
                };

                _context.Connections.Add(connection);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Successfully followed user!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating connection: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return await LoadAvailableUsersAndReturnView(User.IsInRole("Admin"));
            }
        }
        private async Task<IActionResult> LoadAvailableUsersAndReturnView(bool isAdmin = false)
        {
            var currentMemberId = await GetCurrentMemberId();

            var allMembers = await _context.Members
                .Select(m => new BasicMemberDTO
                {
                    MemberId = m.MemberId,
                    MemberName = m.MemberName
                })
                .ToListAsync();

            var availableUsers = isAdmin && currentMemberId == null
                ? allMembers
                : allMembers.Where(m => m.MemberId != currentMemberId).ToList();

            if (currentMemberId != null)
            {
                var followingIds = await _context.Connections
                    .Where(c => c.FollowerId == currentMemberId)
                    .Select(c => c.FollowingId)
                    .ToListAsync();

                availableUsers = availableUsers
                    .Where(m => !followingIds.Contains(m.MemberId))
                    .ToList();
            }

            var viewModel = new ConnectionCreateViewModel
            {
                IsAdmin = isAdmin,
                CurrentMemberId = currentMemberId,
                AllMembers = allMembers,
                AvailableUsers = availableUsers
            };

            return View("Create", viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            bool isAdmin = User.IsInRole("Admin");

            var currentMemberId = await GetCurrentMemberId();

            if (!isAdmin && currentMemberId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var connection = await _context.Connections
                .Include(c => c.Follower)
                .Include(c => c.Following)
                .FirstOrDefaultAsync(c => c.ConnectionId == id);

            if (connection == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin") && connection.FollowerId != currentMemberId)
            {
                return Forbid();
            }

            return View(connection);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool isAdmin = User.IsInRole("Admin");

            var currentMemberId = await GetCurrentMemberId();

            if (!isAdmin && currentMemberId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var connection = await _context.Connections.FindAsync(id);
            if (connection == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin") && connection.FollowerId != currentMemberId)
            {
                return Forbid();
            }

            _context.Connections.Remove(connection);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}