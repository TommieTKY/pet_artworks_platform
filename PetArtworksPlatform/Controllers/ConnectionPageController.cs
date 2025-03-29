using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetArtworksPlatform.Data;
using PetArtworksPlatform.Models;
using System.Threading.Tasks;

namespace PetArtworksPlatform.Controllers
{
    public class ConnectionPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConnectionPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var connections = await _context.Connections
                .Include(c => c.Follower)
                .Include(c => c.Following)
                .ToListAsync();

            return View(connections);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Connection connection)
        {
            if (ModelState.IsValid)
            {
                var existingConnection = _context.Connections
                    .FirstOrDefault(c => c.FollowerId == connection.FollowerId && c.FollowingId == connection.FollowingId);

                if (existingConnection != null)
                {
                    ModelState.AddModelError(string.Empty, "This connection already exists.");
                    return View(connection);
                }

                _context.Connections.Add(connection);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(connection);
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var connection = _context.Connections
                .Include(c => c.Follower)
                .Include(c => c.Following)
                .FirstOrDefault(c => c.ConnectionId == id);

            if (connection == null)
            {
                return NotFound();
            }

            return View(connection);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var connection = _context.Connections.Find(id);
            if (connection != null)
            {
                _context.Connections.Remove(connection);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}