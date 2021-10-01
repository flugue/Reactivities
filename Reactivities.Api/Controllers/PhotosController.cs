using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Interfaces;
using Reactivities.Domain;
using Reactivities.Persistence;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Reactivities.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        public PhotosController(DataContext context,
            UserManager<AppUser> userManager,IPhotoAccessor photoAccessor)
        {
            Context = context;
            UserManager = userManager;
            PhotoAccessor = photoAccessor;
        }

        public DataContext Context { get; }
        public UserManager<AppUser> UserManager { get; }
        public IPhotoAccessor PhotoAccessor { get; }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] IFormFile file)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await Context.Users.Include(p=>p.Photos).FirstOrDefaultAsync(u=>u.UserName == userName);
            if (user == null)
                return NotFound();

            var photoUploadResult = await PhotoAccessor.AddPhoto(file);

            var photo = new Photo
            {
                Url = photoUploadResult.Url,
                Id = photoUploadResult.PublidId
            };

            if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

            user.Photos.Add(photo);

            var result = await Context.SaveChangesAsync() > 0;

            return result ? Ok(photo) : UnprocessableEntity("Problem Uploading");

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await Context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
                return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == id);
            if (photo == null)  return NotFound();

            if (photo.IsMain) return Problem("You cannot delete your main photo");
            var result = await PhotoAccessor.DeletePhoto(photo.Id);

            if (result == null) return NotFound("Problem deleting photo in Cloudinary");

            user.Photos.Remove(photo);
            var success = await Context.SaveChangesAsync() > 0;

            return success ? Ok() : Problem();
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMain(string id)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await Context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
                return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == id);
            if (photo == null) return NotFound();

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            var success = await Context.SaveChangesAsync() > 0;

            return success ? Ok() : Problem();

        }
    }
}
