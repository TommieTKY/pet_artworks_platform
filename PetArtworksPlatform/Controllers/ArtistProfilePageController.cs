using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PetArtworksPlatform.Models;

namespace PetArtworksPlatform.Controllers
{
    public class ArtistProfilePageController : Controller
    {
        private readonly ArtistProfileController _api;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArtistProfilePageController(ArtistProfileController api, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _api = api;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        // GET: ArtistProfilePage/Details/{id} -> A webpage that displays an artist by the Artist’s ID
        [HttpGet]
        [Authorize(Roles = "ArtistUser")]
        public async Task<IActionResult> Details()
        {
            var selectedArtist = (await _api.FindArtist()).Value;
            if (selectedArtist == null)
            {
                return RedirectToAction("New");
            }
            return View(selectedArtist);
        }


        // GET: ArtistProfilePage/New -> A webpage that prompts the user to enter new artist information
        [HttpGet]
        [Authorize(Roles = "ArtistUser")]
        public async Task<IActionResult> New()
        {
            var selectedArtist = (await _api.FindArtist()).Value;
            if (selectedArtist != null)
            {
                return RedirectToAction("Details");
            }

            return View();
        }


        // POST: ArtistProfilePage/Create -> Handles the creation of a new artist
        [HttpPost]
        [Authorize(Roles = "ArtistUser")]
        public async Task<IActionResult> Create(string artistName, string artistBiography)
        {
            var newArtist = new ArtistPersonDto
            {
                ArtistName = artistName,
                ArtistBiography = artistBiography
            };

            var result = await _api.CreateArtist(newArtist);

            if (result.Result == null)
            {
                return View("Error");
            }

            var artist = ((CreatedResult)result.Result).Value;

            if (artist == null)
            {
                return View("Error");
            }

            return RedirectToAction("Details", new { id = ((Artist)artist).ArtistID });
        }

        // GET: ArtistProfilePage/Edit/{id} -> A webpage that prompts the user to edit artist information
        [HttpGet]
        [Authorize(Roles = "ArtistUser")]
        public IActionResult Edit()
        {
            var selectedArtist = _api.FindArtist().Result.Value;
            return View(selectedArtist);
        }

        // POST: ArtistProfilePage/Update/{id} -> Handles the update of an artist's information
        [HttpPost]
        [Authorize(Roles = "ArtistUser")]
        public async Task<IActionResult> Update(string artistName, string artistBiography)
        {
            var updateArtist = new ArtistPersonDto
            {
                //ArtistId = id,
                ArtistName = artistName,
                ArtistBiography = artistBiography
            };

            var result = await _api.UpdateArtist(updateArtist);

            if (result is NoContentResult)
            {
                return RedirectToAction("Details");
            }
            return View("Error");
        }
    }
}
