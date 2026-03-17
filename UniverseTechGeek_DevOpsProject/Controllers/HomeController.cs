using Microsoft.AspNetCore.Mvc;
using Universetechgeek.Models;
using Universetechgeek.Services;

namespace Universetechgeek.Controllers
{
    public class HomeController : Controller
    {
        private readonly MediaApiService _media;

        public HomeController(MediaApiService media)
        {
            _media = media;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _media.GetTrendingBooksAsync(5);
            var games = await _media.GetTrendingGamesAsync(5);
            var movies = await _media.GetTrendingMoviesAsync(5);
            var tvShows = await _media.GetTrendingTvShowsAsync(5);
            var animes = await _media.GetTrendingAnimesAsync(5);

            var model = new HomeViewModel
            {
                Books = books,
                Games = games,
                Movies = movies,
                TvShows = tvShows,
                Animes = animes
            };

            return View(model);
        }
    }
}
