using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetArtworksPlatform.Models.DTOs;
using PetArtworksPlatform.Data;
using System.Threading.Tasks;
using PetArtworksPlatform.Models;
using System.Linq;

namespace PetArtworksPlatform.Controllers
{
    public class MemberPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemberPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
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

        public async Task<IActionResult> Details(int id)
        {
            var member = await _context.Members
                .Where(m => m.MemberId == id)
                .Select(m => new MemberDTO
                {
                    MemberId = m.MemberId,
                    MemberName = m.MemberName,
                    Email = m.Email,
                    Bio = m.Bio,
                    Location = m.Location
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

        public async Task<IActionResult> Edit(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null) return NotFound();

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
        public async Task<IActionResult> Edit(int id, MemberDTO memberDto)
        {
            if (id != memberDto.MemberId) return BadRequest();

            if (!ModelState.IsValid) return View(memberDto);

            var member = await _context.Members.FindAsync(id);
            if (member == null) return NotFound();

            member.MemberName = memberDto.MemberName;
            member.Email = memberDto.Email;
            member.Bio = memberDto.Bio;         
            member.Location = memberDto.Location;

            _context.Members.Update(member);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.Members
                .Where(m => m.MemberId == id)
                .Select(m => new MemberDTO
                {
                    MemberId = m.MemberId,
                    MemberName = m.MemberName
                })
                .FirstOrDefaultAsync();

            if (member == null) return NotFound();
            return View(member);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null) return NotFound();

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
