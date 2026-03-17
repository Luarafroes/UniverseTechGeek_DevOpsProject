namespace Universetechgeek.Models.ApiModels
{
    public class TmdbResponse<T>
    {
        public List<T> Results { get; set; } = new();
    }

    public class TmdbMovie
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Poster_Path { get; set; }
        public double Vote_Average { get; set; }
        public string Overview { get; set; } = "";
    }

    public class TmdbTvShow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Poster_Path { get; set; }
        public double Vote_Average { get; set; }
        public string Overview { get; set; } = "";
    }
}
