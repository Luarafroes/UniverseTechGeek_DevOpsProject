namespace Universetechgeek.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public double Rating { get; set; }
        public string Description { get; set; } = "";
    }
}
