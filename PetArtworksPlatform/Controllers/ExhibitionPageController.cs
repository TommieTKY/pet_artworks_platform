using Microsoft.AspNetCore.Mvc;
using PetArtworksPlatform.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using PetArtworksPlatform.Models.ViewModels;

namespace PetArtworksPlatform.Controllers
{
    public class ExhibitionPageController : Controller
    {
        private readonly ExhibitionsController _exhibitionsApi;
        private readonly ArtworksController _artworksApi;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExhibitionPageController(ExhibitionsController exhibitionsApi, ArtworksController artworksApi, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _exhibitionsApi = exhibitionsApi;
            _artworksApi = artworksApi;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: ExhibitionPage/List -> A webpage that shows all exhibitions in the db
        [HttpGet]
        public async Task<IActionResult> List(int PageNum = 0)
        {
            int PerPage = 5;
            int MaxPage = (int)Math.Ceiling((decimal)await _exhibitionsApi.CountExhibitions() / PerPage) - 1;

            if (MaxPage < 0) MaxPage = 0;
            if (PageNum < 0) PageNum = 0;
            if (PageNum > MaxPage) PageNum = MaxPage;

            int StartIndex = PerPage * PageNum;

            var result = await _exhibitionsApi.List(StartIndex, PerPage);
            IEnumerable<ExhibitionToListDto> ExhibitionList = result.Value;

            ExhibitionList ViewModel = new ExhibitionList();
            ViewModel.Exhibitions = ExhibitionList;
            ViewModel.MaxPage = MaxPage;
            ViewModel.Page = PageNum;

            IdentityUser? User = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (User != null) ViewModel.isAdmin = await _userManager.IsInRoleAsync(User, "Admin");
            else ViewModel.isAdmin = false;

            return View(ViewModel);
        }

        // GET: ExhibitionPage/Details/{id} -> A webpage that displays an exhibition by the Exhibition’s ID
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var selectedExhibition = (await _exhibitionsApi.FindExhibition(id)).Value;
            return View(selectedExhibition);
        }

        // GET: ExhibitionPage/New -> A webpage that prompts the user to enter new exhibition information
        [HttpGet]
        [Authorize(Roles = "Admin,GalleryAdmin")]
        public IActionResult New()
        {
            return View();
        }

        // POST: ExhibitionPage/Create -> Creates a new exhibition
        [HttpPost]
        [Authorize(Roles = "Admin,GalleryAdmin")]
        public async Task<IActionResult> Create(string exhibitionTitle, string exhibitionDescription, DateTime startDate, DateTime endDate)
        {
            var newExhibition = new ExhibitionItemDto
            {
                ExhibitionTitle = exhibitionTitle,
                ExhibitionDescription = exhibitionDescription,
                StartDate = DateOnly.FromDateTime(startDate),
                EndDate = DateOnly.FromDateTime(endDate)
            };

            var result = await _exhibitionsApi.AddExhibition(newExhibition);
            if (result.Result == null)
            {
                return View("Error");
            }

            var createdResult = result.Result as CreatedAtActionResult;
            if (createdResult?.Value == null)
            {
                return View("Error");
            }

            var exhibition = createdResult.Value as Exhibition;
            if (exhibition == null)
            {
                return View("Error");
            }
            return RedirectToAction("Details", new { id = exhibition.ExhibitionID });
        }

        // GET: ExhibitionPage/ConfirmDelete/{id} -> A webpage that confirms the deletion of an exhibition
        [HttpGet]
        [Authorize(Roles = "Admin,GalleryAdmin")]
        public IActionResult ConfirmDelete(int id)
        {
            var selectedExhibition = _exhibitionsApi.FindExhibition(id).Result.Value;
            return View(selectedExhibition);
        }

        // POST: ExhibitionPage/Delete/{id} -> Deletes an exhibition
        [HttpPost]
        [Authorize(Roles = "Admin,GalleryAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _exhibitionsApi.DeleteExhibition(id);
            return RedirectToAction("List");
        }

        // GET: ExhibitionPage/Edit/{id} -> A webpage that prompts the user to edit exhibition information
        [HttpGet]
        [Authorize(Roles = "Admin,GalleryAdmin")]
        public async Task<IActionResult> Edit(int id)
        {
            var selectedExhibition = await _exhibitionsApi.FindExhibition(id);
            if (selectedExhibition.Value is ExhibitionItemDto exhibitionItemDto)
            {
                var artworks = await _artworksApi.List(0,int.MaxValue);
                var exhibitionDetails = new ViewExhibitionEdit
                {
                    Exhibition = exhibitionItemDto,
                    ArtworkList = artworks.Value.ToList()
                };
                return View(exhibitionDetails);
            }
            return View("Error");
        }

        // POST: ExhibitionPage/Update -> Handles the update of an exhibition's information
        [HttpPost]
        [Authorize(Roles = "Admin,GalleryAdmin")]
        public async Task<IActionResult> Update(ViewExhibitionEdit model)
        {
            if (model == null || model.Exhibition == null)
            {
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            var exhibition = model.Exhibition;

            if (string.IsNullOrWhiteSpace(exhibition.ExhibitionTitle) || string.IsNullOrWhiteSpace(exhibition.ExhibitionDescription))
            {
                ModelState.AddModelError(string.Empty, "Title and Description cannot be empty.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            if (exhibition.StartDate == default || exhibition.EndDate == default)
            {
                ModelState.AddModelError(string.Empty, "Start Date and End Date must be valid dates.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            var result = await _exhibitionsApi.UpdateExhibition(exhibition.ExhibitionId, exhibition);
            if (result is NoContentResult)
            {
                return RedirectToAction("Details", new { id = exhibition.ExhibitionId });
            }

            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View("Error", errorViewModel);
        }

        // POST: ExhibitionPage/AddArtwork -> Adds an artwork to an exhibition
        [HttpPost]
        [Authorize(Roles = "Admin,GalleryAdmin")]
        public async Task<IActionResult> AddArtwork(int exhibitionId, int newArtworkId)
        {
            var artworkIdDto = new ArtworkIdDto { ArtworkId = newArtworkId };
            var result = await _exhibitionsApi.AddArtworkToExhibition(exhibitionId, artworkIdDto);

            if (result.Result is NotFoundResult)
            {
                return View("Error");
            }

            return RedirectToAction("Edit", new { id = exhibitionId });
        }

        // POST: ExhibitionPage/RemoveArtwork -> Removes an artwork from an exhibition
        [HttpPost]
        [Authorize(Roles = "Admin,GalleryAdmin")]
        public async Task<IActionResult> RemoveArtwork(int exhibitionId, int artworkId)
        {
            var artworkIdDto = new ArtworkIdDto { ArtworkId = artworkId };
            var result = await _exhibitionsApi.DeleteArtworkFromExhibition(exhibitionId, artworkIdDto);

            if (result is NotFoundResult)
            {
                return View("Error");
            }
            return RedirectToAction("Edit", new { id = exhibitionId });
        }
    }
}

