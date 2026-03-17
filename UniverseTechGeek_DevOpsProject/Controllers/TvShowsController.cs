using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universetechgeek.Data;
using Universetechgeek.Models;
using Universetechgeek.Services;

namespace Universetechgeek.Controllers
{
    public class TvShowsController : Controller
    {
        private readonly MediaApiService _media;
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public TvShowsController(MediaApiService media, AppDbContext db, UserManager<AppUser> userManager)
        {
            _media = media;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var shows = await _media.GetTrendingTvShowsAsync(20);

            var mediaIds = shows.Select(s => s.Id).ToList();
            var ratings = await _db.Reviews
                .Where(r => r.MediaType == "TvShow" && mediaIds.Contains(r.MediaId))
                .GroupBy(r => r.MediaId)
                .Select(g => new { MediaId = g.Key, Avg = g.Average(r => r.Stars) })
                .ToListAsync();

            foreach (var show in shows)
            {
                var rating = ratings.FirstOrDefault(r => r.MediaId == show.Id);
                show.Rating = rating != null ? Math.Round(rating.Avg, 1) : 0;
            }

            return View(shows);
        }

        public async Task<IActionResult> Details(int id)
        {
            var show = await _media.GetTvShowByIdAsync(id);
            if (show == null) return NotFound();

            var reviews = await _db.Reviews
                .Include(r => r.User)
                .Where(r => r.MediaId == id && r.MediaType == "TvShow")
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
                        .FirstOrDefaultAsync(r => r.UserId == user.Id && r.MediaId == id && r.MediaType == "TvShow");
                    userRating = rating?.Stars;

                    inWatchlist = await _db.WatchlistItems
                        .AnyAsync(w => w.UserId == user.Id && w.MediaId == id && w.MediaType == "TvShow");
                }
            }

            var model = new MediaDetailsViewModel<TvShow>
            {
                Media = show,
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
            var shows = await _media.GetTrendingTvShowsAsync(20, page);
            var result = shows.Select(s => new { id = s.Id, title = s.Title, imageUrl = s.ImageUrl, rating = s.Rating, sub = "" });
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Json(new List<object>());
            var results = await _media.SearchTvShowsAsync(q);
            return Json(results.Select(s => new { id = s.Id, title = s.Title, imageUrl = s.ImageUrl, rating = s.Rating, sub = "" }));
        }
    }
}
