using Microsoft.AspNetCore.Mvc;
using Universetechgeek.Models;
using Universetechgeek.Services;

namespace Universetechgeek.Controllers
{
    public class MusicViewModel
    {
        public List<MusicItem> Albums { get; set; } = new();
        public List<MusicItem> Singles { get; set; } = new();
    }

    public class MusicController : Controller
    {
        private readonly MediaApiService _media;

        public MusicController(MediaApiService media)
        {
            _media = media;
        }

        public async Task<IActionResult> Index()
        {
            var model = new MusicViewModel
            {
                Singles = await _media.GetTopTracksAsync(10),
                Albums = await _media.GetTopAlbumsAsync(10)
            };

            return View(model);
        }
    }
}