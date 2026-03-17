using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universetechgeek.Data;
using Universetechgeek.Models;
using Universetechgeek.Services;

namespace Universetechgeek.Controllers
{
    public class GamesController : Controller
    {
        private readonly MediaApiService _media;
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public GamesController(MediaApiService media, AppDbContext db, UserManager<AppUser> userManager)
        {
            _media = media;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var games = await _media.GetTrendingGamesAsync(20);

            var mediaIds = games.Select(g => g.Id).ToList();
            var ratings = await _db.Reviews
                .Where(r => r.MediaType == "Game" && mediaIds.Contains(r.MediaId))
                .GroupBy(r => r.MediaId)
                .Select(g => new { MediaId = g.Key, Avg = g.Average(r => r.Stars) })
                .ToListAsync();

            foreach (var game in games)
            {
                var rating = ratings.FirstOrDefault(r => r.MediaId == game.Id);
                game.Rating = rating != null ? Math.Round(rating.Avg, 1) : 0;
            }

            return View(games);
        }

        public async Task<IActionResult> Details(int id)
        {
            var game = await _media.GetGameByIdAsync(id);
            if (game == null) return NotFound();

            var reviews = await _db.Reviews
                .Include(r => r.User)
                .Where(r => r.MediaId == id && r.MediaType == "Game")
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
                        .FirstOrDefaultAsync(r => r.UserId == user.Id && r.MediaId == id && r.MediaType == "Game");
                    userRating = rating?.Stars;

                    inWatchlist = await _db.WatchlistItems
                        .AnyAsync(w => w.UserId == user.Id && w.MediaId == id && w.MediaType == "Game");
                }
            }

            var model = new MediaDetailsViewModel<Game>
            {
                Media = game,
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
            var games = await _media.GetTrendingGamesAsync(20, page);
            var result = games.Select(g => new { id = g.Id, title = g.Title, imageUrl = g.ImageUrl, rating = g.Rating, sub = "" });
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Json(new List<object>());
            var results = await _media.SearchGamesAsync(q);
            return Json(results.Select(g => new { id = g.Id, title = g.Title, imageUrl = g.ImageUrl, rating = g.Rating, sub = "" }));
        }
    }
}
