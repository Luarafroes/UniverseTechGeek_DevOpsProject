namespace Universetechgeek.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public int MediaId { get; set; }
        public string MediaType { get; set; } = "";
        public int Stars { get; set; }
        public AppUser? User { get; set; }
    }
}
