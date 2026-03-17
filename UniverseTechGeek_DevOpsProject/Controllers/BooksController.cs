using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universetechgeek.Data;
using Universetechgeek.Models;
using Universetechgeek.Services;

namespace Universetechgeek.Controllers
{
    public class BooksController : Controller
    {
        private readonly MediaApiService _media;
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public BooksController(MediaApiService media, AppDbContext db, UserManager<AppUser> userManager)
        {
            _media = media;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _media.GetTrendingBooksAsync(20);

            var mediaIds = books.Select(b => b.Id).ToList();
            var ratings = await _db.Reviews
                .Where(r => r.MediaType == "Book" && mediaIds.Contains(r.MediaId))
                .GroupBy(r => r.MediaId)
                .Select(g => new { MediaId = g.Key, Avg = g.Average(r => r.Stars) })
                .ToListAsync();

            foreach (var book in books)
            {
                var rating = ratings.FirstOrDefault(r => r.MediaId == book.Id);
                book.Rating = rating != null ? Math.Round(rating.Avg, 1) : 0;
            }

            return View(books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var books = await _media.GetTrendingBooksAsync(50);
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();

            var reviews = await _db.Reviews
                .Include(r => r.User)
                .Where(r => r.MediaId == id && r.MediaType == "Book")
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
                        .FirstOrDefaultAsync(r => r.UserId == user.Id && r.MediaId == id && r.MediaType == "Book");
                    userRating = rating?.Stars;

                    inWatchlist = await _db.WatchlistItems
                        .AnyAsync(w => w.UserId == user.Id && w.MediaId == id && w.MediaType == "Book");
                }
            }

            var model = new MediaDetailsViewModel<Book>
            {
                Media = book,
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
            var books = await _media.GetTrendingBooksAsync(20, page);
            var result = books.Select(b => new { id = b.Id, title = b.Title, imageUrl = b.ImageUrl, rating = b.Rating, sub = b.Author });
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Json(new List<object>());
            var results = await _media.SearchBooksAsync(q);
            return Json(results.Select(b => new { id = b.Id, title = b.Title, imageUrl = b.ImageUrl, rating = b.Rating, sub = b.Author }));
        }
    }
}
