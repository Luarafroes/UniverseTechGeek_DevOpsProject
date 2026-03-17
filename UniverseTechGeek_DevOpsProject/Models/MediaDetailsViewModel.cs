namespace Universetechgeek.Models
{
    public class MediaDetailsViewModel<T>
    {
        public T Media { get; set; } = default!;
        public List<Review> Reviews { get; set; } = new();
        public double AverageRating { get; set; }
        public int? UserRating { get; set; }
        public bool IsInWatchlist { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
