using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetArtworksPlatform.Data;
using PetArtworksPlatform.Models;
using System.Security.Claims;
using System.Threading.Tasks;

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
            if (currentMemberId == null)
            {
                return RedirectToAction("Login", "Account");
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
            var currentMemberId = await GetCurrentMemberId();
            if (currentMemberId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var followingIds = await _context.Connections
                .Where(c => c.FollowerId == currentMemberId)
                .Select(c => c.FollowingId)
                .ToListAsync();

            var availableUsers = await _context.Members
                .Where(m => m.MemberId != currentMemberId && !followingIds.Contains(m.MemberId))
                .Select(m => new { m.MemberId, m.MemberName })
                .ToListAsync();

            ViewBag.AvailableUsers = availableUsers;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int followingId)
        {
            var currentMemberId = await GetCurrentMemberId();
            if (currentMemberId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (currentMemberId == followingId)
            {
                ModelState.AddModelError(string.Empty, "You cannot follow yourself.");
                return await LoadAvailableUsersAndReturnView();
            }

            var existingConnection = await _context.Connections
                .FirstOrDefaultAsync(c => c.FollowerId == currentMemberId &&
                                        c.FollowingId == followingId);

            if (existingConnection != null)
            {
                ModelState.AddModelError(string.Empty, "You are already following this user.");
                return await LoadAvailableUsersAndReturnView();
            }

            var newConnection = new Connection
            {
                FollowerId = currentMemberId.Value,
                FollowingId = followingId
            };

            _context.Connections.Add(newConnection);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private async Task<IActionResult> LoadAvailableUsersAndReturnView()
        {
            var currentMemberId = await GetCurrentMemberId();
            var followingIds = await _context.Connections
                .Where(c => c.FollowerId == currentMemberId)
                .Select(c => c.FollowingId)
                .ToListAsync();

            var availableUsers = await _context.Members
                .Where(m => m.MemberId != currentMemberId && !followingIds.Contains(m.MemberId))
                .Select(m => new { m.MemberId, m.MemberName })
                .ToListAsync();

            ViewBag.AvailableUsers = availableUsers;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var currentMemberId = await GetCurrentMemberId();
            if (currentMemberId == null)
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
            var currentMemberId = await GetCurrentMemberId();
            if (currentMemberId == null)
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