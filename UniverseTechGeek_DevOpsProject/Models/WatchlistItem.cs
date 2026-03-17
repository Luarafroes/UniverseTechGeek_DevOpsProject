namespace Universetechgeek.Models
{
    public class WatchlistItem
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public int MediaId { get; set; }
        public string MediaType { get; set; } = "";
        public string Title { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public DateTime AddedAt { get; set; }
        public AppUser? User { get; set; }
    }
}
