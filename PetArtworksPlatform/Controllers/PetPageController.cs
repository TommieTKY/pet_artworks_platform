using Microsoft.AspNetCore.Mvc;
using PetArtworksPlatform.Data;
using PetArtworksPlatform.Models;
using PetArtworksPlatform.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PetArtworksPlatform.Controllers
{
    public class PetPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PetPageController(ApplicationDbContext context)
        {
            _context = context;
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
                    PetImagePath = pet.HasPic ? $"/image/pet/{pet.PetId}{pet.PicExtension}" : null
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
                .ToList()
            };

            return View(petDetails);
        }

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
        public async Task<IActionResult> Create(PetDTO petDto)
        {
            if (ModelState.IsValid)
            {
                // Validate owners
                foreach (var ownerId in petDto.OwnerIds)
                {
                    var owner = await _context.Members.FindAsync(ownerId);
                    if (owner == null)
                    {
                        ModelState.AddModelError("OwnerIds", $"Owner with ID {ownerId} not found");
                        petDto.OwnerList = await GetOwnerList(); // Repopulate owner list
                        return View(petDto);
                    }
                }

                // Create the pet
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
                await _context.SaveChangesAsync(); // Save first to get the PetId

                // Handle image upload if present
                if (petDto.PetImage != null && petDto.PetImage.Length > 0)
                {
                    // Ensure directory exists
                    var imageDirectory = Path.Combine("wwwroot", "image", "pet");
                    if (!Directory.Exists(imageDirectory))
                    {
                        Directory.CreateDirectory(imageDirectory);
                    }

                    // Save the image
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

            // If we got this far, something failed; repopulate owner list
            petDto.OwnerList = await GetOwnerList();
            return View(petDto);
        }

        // Helper method to get owner list
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
                // Ensure directory exists
                var imageDirectory = Path.Combine("wwwroot", "image", "pet");
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory);
                }

                // Delete old image if exists
                if (pet.HasPic)
                {
                    string oldFilePath = Path.Combine(imageDirectory, $"{pet.PetId}{pet.PicExtension}");
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save new image
                string fileExtension = Path.GetExtension(petDto.PetImage.FileName);
                string fileName = $"{pet.PetId}{fileExtension}";
                string filePath = Path.Combine(imageDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await petDto.PetImage.CopyToAsync(stream);
                }

                // Update pet properties
                pet.HasPic = true;
                pet.PicExtension = fileExtension;
            }

            // Rest of your existing code for owner updates...
            _context.Update(pet);

            var existingOwners = _context.PetOwners
                .Where(po => po.PetId == pet.PetId)
                .ToList();

            _context.PetOwners.RemoveRange(existingOwners.Where(po => !petDto.OwnerIds.Contains(po.OwnerId)));

            var newOwners = petDto.OwnerIds?
                .Where(ownerId => !existingOwners.Any(po => po.OwnerId == ownerId))
                .Select(ownerId => new PetOwner
                {
                    PetId = pet.PetId,
                    OwnerId = ownerId
                }).ToList();

            _context.PetOwners.AddRange(newOwners);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [HttpGet("Delete/{id}")]
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

        private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.PetId == id);
        }

    }
}