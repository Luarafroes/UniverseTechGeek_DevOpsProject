// ── Anime.cs ─────────────────────────────────────────────────
namespace Universetechgeek.Models
{
    public class Anime
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public double Rating { get; set; }
        public string Description { get; set; } = "";
        public string Status { get; set; } = "";
        public int Episodes { get; set; }
        public string Genre { get; set; } = "";
    }
}
