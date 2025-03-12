using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pawpals.Data;
using pawpals.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace pawpals.Controllers
{
    public class HomePageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomePageController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); 
            }

            var member = await _context.Members
                .Include(m => m.Followers)
                .Include(m => m.Following)
                .Include(m => m.PetOwners)
                .FirstOrDefaultAsync(m => m.UserId == userId);

            if (member == null)
            {
                return NotFound(); 
            }

            return View(member);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}