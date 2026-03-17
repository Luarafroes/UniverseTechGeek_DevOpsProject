namespace Universetechgeek.Models
{
    public class HomeViewModel
    {
        public List<Book> Books { get; set; } = new();
        public List<Game> Games { get; set; } = new();
        public List<Movie> Movies { get; set; } = new();
        public List<TvShow> TvShows { get; set; } = new();
        public List<Anime> Animes { get; set; } = new();
    }
}
