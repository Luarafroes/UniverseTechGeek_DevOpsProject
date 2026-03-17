namespace Universetechgeek.Models.ApiModels
{
    public class RawgResponse
    {
        public List<RawgGame> Results { get; set; } = new();
    }

    public class RawgGame
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Background_Image { get; set; }
        public double Rating { get; set; }
        public string? Released { get; set; }
    }
}
