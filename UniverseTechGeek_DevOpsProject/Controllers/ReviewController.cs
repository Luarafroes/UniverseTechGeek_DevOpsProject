using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universetechgeek.Data;
using Universetechgeek.Models;

namespace Universetechgeek.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public ReviewController(AppDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int mediaId, string mediaType, string? content, int stars)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var existing = await _db.Reviews
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.MediaId == mediaId && r.MediaType == mediaType);

            if (existing != null)
            {
                existing.Stars = stars;
                existing.Content = content ?? "";
                existing.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                _db.Reviews.Add(new Review
                {
                    UserId = user.Id,
                    MediaId = mediaId,
                    MediaType = mediaType,
                    Content = content ?? "",
                    Stars = stars,
                    CreatedAt = DateTime.UtcNow
                });
            }

            var existingRating = await _db.Ratings
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.MediaId == mediaId && r.MediaType == mediaType);

            if (existingRating != null)
                existingRating.Stars = stars;
            else
                _db.Ratings.Add(new Rating
                {
                    UserId = user.Id,
                    MediaId = mediaId,
                    MediaType = mediaType,
                    Stars = stars
                });

            await _db.SaveChangesAsync();

            return RedirectToAction("Details", mediaType + "s", new { id = mediaId });
        }

        [HttpPost]
        public async Task<IActionResult> AddToWatchlist(int mediaId, string mediaType, string title, string imageUrl)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var exists = await _db.WatchlistItems
                .AnyAsync(w => w.UserId == user.Id && w.MediaId == mediaId && w.MediaType == mediaType);

            if (!exists)
            {
                _db.WatchlistItems.Add(new WatchlistItem
                {
                    UserId = user.Id,
                    MediaId = mediaId,
                    MediaType = mediaType,
                    Title = title,
                    ImageUrl = imageUrl,
                    AddedAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Details", mediaType + "s", new { id = mediaId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWatchlist(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var item = await _db.WatchlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == user.Id);

            if (item != null)
            {
                _db.WatchlistItems.Remove(item);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Profile", "Account");
        }
    }
}
