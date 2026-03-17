using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universetechgeek.Data;
using Universetechgeek.Models;
using Universetechgeek.Services;

namespace UniverseTechGeek_DevOpsProject.Controllers
{
    public class AnimesController : Controller
    {
        private readonly MediaApiService _media;
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public AnimesController(MediaApiService media, AppDbContext db, UserManager<AppUser> userManager)
        {
            _media = media;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var animes = await _media.GetTrendingAnimesAsync(20);

            var mediaIds = animes.Select(a => a.Id).ToList();
            var ratings = await _db.Reviews
                .Where(r => r.MediaType == "Anime" && mediaIds.Contains(r.MediaId))
                .GroupBy(r => r.MediaId)
                .Select(g => new { MediaId = g.Key, Avg = g.Average(r => r.Stars) })
                .ToListAsync();

            foreach (var anime in animes)
            {
                var rating = ratings.FirstOrDefault(r => r.MediaId == anime.Id);
                anime.Rating = rating != null ? Math.Round(rating.Avg, 1) : 0;
            }

            return View(animes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var anime = await _media.GetAnimeByIdAsync(id);
            if (anime == null) return NotFound();

            var reviews = await _db.Reviews
                .Include(r => r.User)
                .Where(r => r.MediaId == id && r.MediaType == "Anime")
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var avgRating = reviews.Any() ? reviews.Average(r => r.Stars) : 0;

            int? userRating = null;
            bool inWatchlist = false;

            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var rating = await _db.Ratings
                        .FirstOrDefaultAsync(r => r.UserId == user.Id && r.MediaId == id && r.MediaType == "Anime");
                    userRating = rating?.Stars;

                    inWatchlist = await _db.WatchlistItems
                        .AnyAsync(w => w.UserId == user.Id && w.MediaId == id && w.MediaType == "Anime");
                }
            }

            var model = new MediaDetailsViewModel<Anime>
            {
                Media = anime,
                Reviews = reviews,
                AverageRating = Math.Round(avgRating, 1),
                UserRating = userRating,
                IsInWatchlist = inWatchlist,
                IsLoggedIn = User.Identity?.IsAuthenticated == true
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMore(int page = 2)
        {
            var animes = await _media.GetTrendingAnimesAsync(20, page);
            var result = animes.Select(a => new { id = a.Id, title = a.Title, imageUrl = a.ImageUrl, rating = a.Rating, sub = a.Genre });
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Json(new List<object>());
            var results = await _media.SearchAnimesAsync(q);
            return Json(results.Select(a => new { id = a.Id, title = a.Title, imageUrl = a.ImageUrl, rating = a.Rating, sub = a.Genre }));
        }
    }
}
