using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetArtworksPlatform.Data;
using PetArtworksPlatform.Models;

namespace PetArtworksPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ArtistProfileController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet(template: "FindArtist")]
        [Authorize(Roles = "ArtistUser")]
        public async Task<ActionResult<ArtistPersonDto>> FindArtist()
        {
            IdentityUser? User = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            string currentId = User.Id;

            Artist Artist = await _context.Artists
            .Include(a => a.ArtistUser)
            .Include(a => a.Artworks)
            .FirstOrDefaultAsync(a => a.ArtistUser.Id == currentId);

            if (Artist == null)
            {
                return NotFound("Artist not found. Please create a new artist.");
            }

            if (Artist.ArtistUser?.Id != currentId)
            {
                return Forbid();
            }

            ArtistPersonDto ArtistDto = new ArtistPersonDto();

            ArtistDto.ArtistId = Artist.ArtistID;
            ArtistDto.ArtistName = Artist.ArtistName;
            ArtistDto.ArtistBiography = Artist.ArtistBiography;
            ArtistDto.ListArtworks = Artist.Artworks?.Select(a => new ArtworkForOtherDto { ArtworkId = a.ArtworkID, ArtworkTitle = a.ArtworkTitle }).ToList();

            return ArtistDto;
        }

        [HttpPost(template: "CreateArtist")]
        [Authorize(Roles = "ArtistUser")]
        public async Task<ActionResult<Artist>> CreateArtist([FromBody] ArtistPersonDto artistDto)
        {
            IdentityUser? User = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            string currentId = User.Id;

            bool artistExists = await _context.Artists.AnyAsync(a => a.ArtistUser.Id == currentId);
            if (artistExists)
            {
                return BadRequest(new { message = "Artist profile already exists." });
            }

            if (string.IsNullOrWhiteSpace(artistDto.ArtistName) || string.IsNullOrWhiteSpace(artistDto.ArtistBiography))
            {
                return BadRequest(new { message = "Invalid artist data" });
            }

            Artist artist = new Artist
            {
                ArtistName = artistDto.ArtistName,
                ArtistBiography = artistDto.ArtistBiography,
                ArtistUser = User,

            };

            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            return Created($"api/ArtistProfile/FindArtist/{artist.ArtistID}", artist);
        }

        [HttpPut(template: "UpdateArtist")]
        [Authorize(Roles = "ArtistUser")]
        public async Task<IActionResult> UpdateArtist([FromBody] ArtistPersonDto artistDto)
        {
            IdentityUser? User = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            string currentId = User.Id;

            //Artist Artist = await _context.Artists
            //.Include(a => a.ArtistUser)
            //.FirstOrDefaultAsync(a => a.ArtistID == ArtistID);

            Artist Artist = await _context.Artists
            .Include(a => a.ArtistUser)
            .FirstOrDefaultAsync(a => a.ArtistUser.Id == currentId);

            if (Artist == null)
            {
                return NotFound();
            }

            if (Artist.ArtistUser?.Id != currentId)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(artistDto.ArtistName) || string.IsNullOrWhiteSpace(artistDto.ArtistBiography))
            {
                return BadRequest(new { message = "Invalid artist data" });
            }

            Artist.ArtistName = artistDto.ArtistName;
            Artist.ArtistBiography = artistDto.ArtistBiography;

            _context.Entry(Artist).State = EntityState.Modified;

            try
            {
                // SQL Equivalent: Update artists set ... where Artist={ArtistID}
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ArtistExists())
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

        private async Task<bool> ArtistExists()
        {
            IdentityUser? User = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            string currentId = User.Id;
            return await _context.Artists.AnyAsync(a => a.ArtistUser.Id == currentId);
        }
    }
}
