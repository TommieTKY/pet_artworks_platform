using Microsoft.AspNetCore.Mvc;
using PetArtworksPlatform.Data;
using PetArtworksPlatform.Models;
using PetArtworksPlatform.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace PetArtworksPlatform.Controllers
{
    public class PetPageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ArtworksController _artworksApi;
        private readonly PetController _petApi;

        public PetPageController(ApplicationDbContext context, ArtworksController artworksApi, PetController petApi)
        {
            _context = context;
            _artworksApi = artworksApi;
            _petApi = petApi;
        }

        public async Task<IActionResult> Index()
        {
            var pets = await _context.Pets.ToListAsync();
            return RedirectToAction("List");
        }

        public async Task<IActionResult> List()
        {
            var petDTOs = await _context.Pets
                .Include(p => p.PetOwners)
                .Select(p => new PetDTO
                {
                    PetId = p.PetId,
                    Name = p.Name,
                    Type = p.Type,
                    Breed = p.Breed,
                    DOB = p.DOB,
                    OwnerIds = p.PetOwners.Select(po => po.OwnerId).ToList(),
                    HasPic = p.HasPic,
                    PetImagePath = p.HasPic ? $"/image/pet/{p.PetId}{p.PicExtension}" : null
                })
                .ToListAsync();

            return View(petDTOs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.Artworks)
                .Include(p => p.PetOwners)
                    .ThenInclude(po => po.Owner)
                .FirstOrDefaultAsync(m => m.PetId == id);

            if (pet == null)
            {
                return NotFound();
            }

            var petDetails = new PetDetails
            {
                Pet = new PetDTO
                {
                    PetId = pet.PetId,
                    Name = pet.Name ?? string.Empty,
                    Type = pet.Type ?? string.Empty,
                    Breed = pet.Breed ?? string.Empty,
                    DOB = pet.DOB,
                    OwnerIds = pet.PetOwners.Select(po => po.OwnerId).ToList(),
                    HasPic = pet.HasPic,
                    PetImagePath = pet.HasPic ? $"/image/pet/{pet.PetId}{pet.PicExtension}" : null,
                    ListArtworks = pet.Artworks?.Select(p => new ArtworkForOtherDto
                    {
                        ArtworkId = p.ArtworkID,
                        ArtworkTitle = p.ArtworkTitle
                    }).ToList() ?? new List<ArtworkForOtherDto>()
                },
                Owners = pet.PetOwners
                .Select(po => new MemberDTO
                {
                    MemberId = po.Owner.MemberId,
                    MemberName = po.Owner.MemberName,
                    Email = po.Owner.Email,
                    Bio = po.Owner.Bio,
                    Location = po.Owner.Location
                })
                .ToList(),
                OwnerList = await _context.Members
                    .Select(m => new MemberDTO
                    {
                        MemberId = m.MemberId,
                        MemberName = m.MemberName
                    })
                    .ToListAsync(),
                ArtworkList = (await _artworksApi.List()).Value?.Select(a => new ArtworkToListDto
                {
                    ArtworkId = a.ArtworkId,
                    ArtworkTitle = a.ArtworkTitle
                }).ToList() ?? new List<ArtworkToListDto>()
            };

            return View(petDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UpdateOwners(int petId, List<int> ownerIds)
        {
            var pet = await _context.Pets
                .Include(p => p.PetOwners)
                .FirstOrDefaultAsync(p => p.PetId == petId);

            if (pet == null)
            {
                return NotFound();
            }

            // Update owners
            var existingOwners = pet.PetOwners.ToList();

            // Remove owners that are not in the new list
            _context.PetOwners.RemoveRange(existingOwners.Where(po => !ownerIds.Contains(po.OwnerId)));

            // Add new owners
            var newOwners = ownerIds?
                .Where(ownerId => !existingOwners.Any(po => po.OwnerId == ownerId))
                .Select(ownerId => new PetOwner
                {
                    PetId = petId,
                    OwnerId = ownerId
                }).ToList();

            if (newOwners != null && newOwners.Any())
            {
                _context.PetOwners.AddRange(newOwners);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = petId });
        }

        [Authorize]
        public IActionResult Create()
        {
            var owners = _context.Members
                .Select(m => new MemberDTO
                {
                    MemberId = m.MemberId,
                    MemberName = m.MemberName
                })
                .ToList();

            var petDto = new PetDTO
            {
                OwnerList = owners,
                OwnerIds = new List<int>()
            };

            return View(petDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(PetDTO petDto)
        {
            if (ModelState.IsValid)
            {
                foreach (var ownerId in petDto.OwnerIds)
                {
                    var owner = await _context.Members.FindAsync(ownerId);
                    if (owner == null)
                    {
                        ModelState.AddModelError("OwnerIds", $"Owner with ID {ownerId} not found");
                        petDto.OwnerList = await GetOwnerList();
                        return View(petDto);
                    }
                }

                var pet = new Pet
                {
                    Name = petDto.Name,
                    Type = petDto.Type,
                    Breed = petDto.Breed,
                    DOB = petDto.DOB,
                    HasPic = petDto.PetImage != null,
                    PicExtension = petDto.PetImage != null ? Path.GetExtension(petDto.PetImage.FileName) : null
                };

                _context.Add(pet);
                await _context.SaveChangesAsync(); 

                if (petDto.PetImage != null && petDto.PetImage.Length > 0)
                {
                    var imageDirectory = Path.Combine("wwwroot", "image", "pet");
                    if (!Directory.Exists(imageDirectory))
                    {
                        Directory.CreateDirectory(imageDirectory);
                    }

                    string fileName = $"{pet.PetId}{pet.PicExtension}";
                    string filePath = Path.Combine(imageDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await petDto.PetImage.CopyToAsync(stream);
                    }
                }

                // Add owners
                foreach (var ownerId in petDto.OwnerIds)
                {
                    var petOwner = new PetOwner
                    {
                        PetId = pet.PetId,
                        OwnerId = ownerId
                    };
                    _context.PetOwners.Add(petOwner);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(List));
            }

            petDto.OwnerList = await GetOwnerList();
            return View(petDto);
        }

        private async Task<List<MemberDTO>> GetOwnerList()
        {
            return await _context.Members
                .Select(m => new MemberDTO
                {
                    MemberId = m.MemberId,
                    MemberName = m.MemberName
                })
                .ToListAsync();
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.PetOwners)
                .FirstOrDefaultAsync(p => p.PetId == id);

            if (pet == null)
            {
                return NotFound();
            }

            var petDto = new PetDTO
            {
                PetId = pet.PetId,
                Name = pet.Name,
                Type = pet.Type,
                Breed = pet.Breed,
                DOB = pet.DOB,
                OwnerIds = pet.PetOwners?.Select(po => po.OwnerId).ToList() ?? new List<int>(),
                OwnerList = await _context.Members
                    .Select(m => new MemberDTO
                    {
                        MemberId = m.MemberId,
                        MemberName = m.MemberName
                    })
                    .ToListAsync(),
                HasPic = pet.HasPic,
                PetImagePath = pet.HasPic ? $"/image/pet/{pet.PetId}{pet.PicExtension}" : null
            };
            return View(petDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, PetDTO petDto)
        {
            if (id != petDto.PetId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                petDto.OwnerList = await _context.Members
                    .Select(m => new MemberDTO
                    {
                        MemberId = m.MemberId,
                        MemberName = m.MemberName
                    })
                    .ToListAsync();

                return View(petDto);
            }

            var pet = await _context.Pets
                .Include(p => p.PetOwners)
                .FirstOrDefaultAsync(p => p.PetId == id);

            if (pet == null)
            {
                return NotFound();
            }

            pet.Name = petDto.Name;
            pet.Type = petDto.Type;
            pet.Breed = petDto.Breed;
            pet.DOB = petDto.DOB;

            if (petDto.PetImage != null && petDto.PetImage.Length > 0)
            {
                var imageDirectory = Path.Combine("wwwroot", "image", "pet");
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory);
                }

                if (pet.HasPic)
                {
                    string oldFilePath = Path.Combine(imageDirectory, $"{pet.PetId}{pet.PicExtension}");
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                string fileExtension = Path.GetExtension(petDto.PetImage.FileName);
                string fileName = $"{pet.PetId}{fileExtension}";
                string filePath = Path.Combine(imageDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await petDto.PetImage.CopyToAsync(stream);
                }

                pet.HasPic = true;
                pet.PicExtension = fileExtension;
            }

            _context.Update(pet);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [HttpGet("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.PetOwners)
                .FirstOrDefaultAsync(p => p.PetId == id);

            if (pet == null)
            {
                return NotFound();
            }

            var petDto = new PetDTO
            {
                PetId = pet.PetId,
                Name = pet.Name,
                Type = pet.Type,
                Breed = pet.Breed,
                DOB = pet.DOB,
                OwnerIds = pet.PetOwners.Select(po => po.OwnerId).ToList()
            };

            return View(petDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _context.Pets
                .Include(p => p.PetOwners)
                .FirstOrDefaultAsync(p => p.PetId == id);

            if (pet == null)
            {
                return NotFound();
            }

            _context.PetOwners.RemoveRange(pet.PetOwners);

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddArtwork(int petId, int newArtworkId)
        {
            var artworkIdDto = new ArtworkIdDto { ArtworkId = newArtworkId };
            var result = await _petApi.AddArtworkToPet(petId, artworkIdDto);

            if (result.Result is NotFoundResult)
            {
                return View("Error");
            }

            return RedirectToAction("Details", new { id = petId });
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveArtwork(int petId, int artworkId)
        {
            var artworkIdDto = new ArtworkIdDto { ArtworkId = artworkId };
            var result = await _petApi.DeleteArtworkFromPet(petId, artworkIdDto);

            if (result is NotFoundResult)
            {
                return View("Error");
            }
            return RedirectToAction("Details", new { id = petId });
        }

        private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.PetId == id);
        }

    }
}