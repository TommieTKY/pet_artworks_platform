using Microsoft.AspNetCore.Mvc;
using PetArtworksPlatform.Models;
using Azure;
using Microsoft.AspNetCore.Authorization;
using PetArtworksPlatform.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace PetArtworksPlatform.Controllers
{
    public class ArtistPageController : Controller
    {
        private readonly ArtistsController _api;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ArtistPageController(ArtistsController api, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _api = api;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: ArtistPage/List -> A webpage that shows all artists in the db
        [HttpGet]
        public async Task<IActionResult> List(int PageNum = 0)
        {
            int PerPage = 5;
            int MaxPage = (int)Math.Ceiling((decimal)await _api.CountArtists() / PerPage) - 1;

            if (MaxPage < 0) MaxPage = 0;
            if (PageNum < 0) PageNum = 0;
            if (PageNum > MaxPage) PageNum = MaxPage;

            int StartIndex = PerPage * PageNum;

            var result = await _api.List(StartIndex, PerPage);
            IEnumerable<ArtistToListDto> ArtistList = result.Value;

            ArtistList ViewModel = new ArtistList();
            ViewModel.Artists = ArtistList;
            ViewModel.MaxPage = MaxPage;
            ViewModel.Page = PageNum;

            IdentityUser? User = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (User != null) ViewModel.isAdmin = await _userManager.IsInRoleAsync(User, "Admin");
            else ViewModel.isAdmin = false;

            return View(ViewModel);
        }

        // GET: ArtistPage/Details/{id} -> A webpage that displays an artist by the Artist’s ID
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var selectedArtist = (await _api.FindArtist(id)).Value;
            return View(selectedArtist);
        }

        // GET: ArtistPage/New -> A webpage that prompts the user to enter new artist information
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            return View();
        }

        // POST: ArtistPage/Create -> Handles the creation of a new artist
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(string artistName, string artistBiography)
        {
            var newArtist = new ArtistPersonDto
            {
                ArtistName = artistName,
                ArtistBiography = artistBiography
            };

            var result = await _api.AddArtist(newArtist);

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

        // GET: ArtistPage/ConfirmDelete/{id} -> A webpage that prompts the user to confirm the deletion of an artist
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ConfirmDelete(int id)
        {
            var selectedArtist = _api.FindArtist(id).Result.Value;
            return View(selectedArtist);
        }

        // POST: ArtistPage/Delete/{id} -> Handles the deletion of an artist
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _api.DeleteArtist(id);
            return RedirectToAction("List");
        }

        // GET: ArtistPage/Edit/{id} -> A webpage that prompts the user to edit an artist's information
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var selectedArtist = _api.FindArtist(id).Result.Value;
            return View(selectedArtist);
        }

        // POST: ArtistPage/Update/{id} -> Handles the update of an artist's information
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, string artistName, string artistBiography)
        {
            var updateArtist = new ArtistPersonDto
            {
                ArtistId = id,
                ArtistName = artistName,
                ArtistBiography = artistBiography
            };

            var result = await _api.UpdateArtist(id, updateArtist);

            if (result is NoContentResult)
            {
                return RedirectToAction("Details", new { id = id });
            }
            return View("Error");
        }
    }
}
