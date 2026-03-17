namespace Universetechgeek.Models
{
    public class ProfileViewModel
    {
        public AppUser User { get; set; } = default!;
        public List<Review> Reviews { get; set; } = new();
        public List<WatchlistItem> Watchlist { get; set; } = new();
    }
}
