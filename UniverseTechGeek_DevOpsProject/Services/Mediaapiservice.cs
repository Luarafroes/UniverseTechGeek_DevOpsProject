using System.Text.Json;
using Universetechgeek.Models;
using Universetechgeek.Models.ApiModels;

namespace Universetechgeek.Services
{
    public class MediaApiService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private const string TmdbBase = "https://api.themoviedb.org/3";
        private const string RawgBase = "https://api.rawg.io/api";
        private const string ImgBase = "https://image.tmdb.org/t/p/w500";

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public MediaApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        // ── HELPERS ──────────────────────────────────────────────
        private static string StripHtml(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", "");
        }

        // ── MOVIES ───────────────────────────────────────────────
        public async Task<List<Movie>> GetTrendingMoviesAsync(int count = 20, int page = 1)
        {
            var key = _config["ApiKeys:TMDb"];
            var url = $"{TmdbBase}/trending/movie/week?api_key={key}&language=pt-BR&page={page}";
            var json = await _http.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<TmdbResponse<TmdbMovie>>(json, JsonOpts);

            return data?.Results.Take(count).Select(m => new Movie
            {
                Id = m.Id,
                Title = m.Title,
                ImageUrl = string.IsNullOrEmpty(m.Poster_Path) ? "/img/no-cover.jpg" : $"{ImgBase}{m.Poster_Path}",
                Rating = Math.Round(m.Vote_Average, 1),
                Description = m.Overview
            }).ToList() ?? new();
        }

        public async Task<Movie?> GetMovieByIdAsync(int id)
        {
            var key = _config["ApiKeys:TMDb"];
            var url = $"{TmdbBase}/movie/{id}?api_key={key}&language=pt-BR";
            var json = await _http.GetStringAsync(url);
            var m = JsonSerializer.Deserialize<TmdbMovie>(json, JsonOpts);
            if (m == null) return null;

            return new Movie
            {
                Id = m.Id,
                Title = m.Title,
                ImageUrl = string.IsNullOrEmpty(m.Poster_Path) ? "/img/no-cover.jpg" : $"{ImgBase}{m.Poster_Path}",
                Rating = Math.Round(m.Vote_Average, 1),
                Description = m.Overview
            };
        }

        public async Task<List<Movie>> SearchMoviesAsync(string query)
        {
            var key = _config["ApiKeys:TMDb"];
            var url = $"{TmdbBase}/search/movie?api_key={key}&language=pt-BR&query={Uri.EscapeDataString(query)}";
            var json = await _http.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<TmdbResponse<TmdbMovie>>(json, JsonOpts);

            return data?.Results.Take(20).Select(m => new Movie
            {
                Id = m.Id,
                Title = m.Title,
                ImageUrl = string.IsNullOrEmpty(m.Poster_Path) ? "/img/no-cover.jpg" : $"{ImgBase}{m.Poster_Path}",
                Rating = Math.Round(m.Vote_Average, 1),
                Description = m.Overview
            }).ToList() ?? new();
        }

        // ── TV SHOWS ─────────────────────────────────────────────
        public async Task<List<TvShow>> GetTrendingTvShowsAsync(int count = 20, int page = 1)
        {
            var key = _config["ApiKeys:TMDb"];
            var url = $"{TmdbBase}/trending/tv/week?api_key={key}&language=pt-BR&page={page}";
            var json = await _http.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<TmdbResponse<TmdbTvShow>>(json, JsonOpts);

            return data?.Results.Take(count).Select(s => new TvShow
            {
                Id = s.Id,
                Title = s.Name,
                ImageUrl = string.IsNullOrEmpty(s.Poster_Path) ? "/img/no-cover.jpg" : $"{ImgBase}{s.Poster_Path}",
                Rating = Math.Round(s.Vote_Average, 1),
                Description = s.Overview
            }).ToList() ?? new();
        }

        public async Task<TvShow?> GetTvShowByIdAsync(int id)
        {
            var key = _config["ApiKeys:TMDb"];
            var url = $"{TmdbBase}/tv/{id}?api_key={key}&language=pt-BR";
            var json = await _http.GetStringAsync(url);
            var s = JsonSerializer.Deserialize<TmdbTvShow>(json, JsonOpts);
            if (s == null) return null;

            return new TvShow
            {
                Id = s.Id,
                Title = s.Name,
                ImageUrl = string.IsNullOrEmpty(s.Poster_Path) ? "/img/no-cover.jpg" : $"{ImgBase}{s.Poster_Path}",
                Rating = Math.Round(s.Vote_Average, 1),
                Description = s.Overview
            };
        }

        public async Task<List<TvShow>> SearchTvShowsAsync(string query)
        {
            var key = _config["ApiKeys:TMDb"];
            var url = $"{TmdbBase}/search/tv?api_key={key}&language=pt-BR&query={Uri.EscapeDataString(query)}";
            var json = await _http.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<TmdbResponse<TmdbTvShow>>(json, JsonOpts);

            return data?.Results.Take(20).Select(s => new TvShow
            {
                Id = s.Id,
                Title = s.Name,
                ImageUrl = string.IsNullOrEmpty(s.Poster_Path) ? "/img/no-cover.jpg" : $"{ImgBase}{s.Poster_Path}",
                Rating = Math.Round(s.Vote_Average, 1),
                Description = s.Overview
            }).ToList() ?? new();
        }

        // ── GAMES ────────────────────────────────────────────────
        public async Task<List<Game>> GetTrendingGamesAsync(int count = 20, int page = 1)
        {
            var key = _config["ApiKeys:RAWG"];
            var url = $"{RawgBase}/games?key={key}&ordering=-rating&page_size={count}&page={page}";
            var json = await _http.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<RawgResponse>(json, JsonOpts);

            return data?.Results.Select(g => new Game
            {
                Id = g.Id,
                Title = g.Name,
                ImageUrl = g.Background_Image ?? "/img/no-cover.jpg",
                Rating = Math.Round(g.Rating, 1),
                Description = $"Released: {g.Released}"
            }).ToList() ?? new();
        }

        public async Task<Game?> GetGameByIdAsync(int id)
        {
            var key = _config["ApiKeys:RAWG"];
            var url = $"{RawgBase}/games/{id}?key={key}";
            var json = await _http.GetStringAsync(url);
            var g = JsonSerializer.Deserialize<RawgGame>(json, JsonOpts);
            if (g == null) return null;

            return new Game
            {
                Id = g.Id,
                Title = g.Name,
                ImageUrl = g.Background_Image ?? "/img/no-cover.jpg",
                Rating = Math.Round(g.Rating, 1),
                Description = $"Released: {g.Released}"
            };
        }

        public async Task<List<Game>> SearchGamesAsync(string query)
        {
            var key = _config["ApiKeys:RAWG"];
            var url = $"{RawgBase}/games?key={key}&search={Uri.EscapeDataString(query)}&page_size=20";
            var json = await _http.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<RawgResponse>(json, JsonOpts);

            return data?.Results.Select(g => new Game
            {
                Id = g.Id,
                Title = g.Name,
                ImageUrl = g.Background_Image ?? "/img/no-cover.jpg",
                Rating = Math.Round(g.Rating, 1),
                Description = $"Released: {g.Released}"
            }).ToList() ?? new();
        }

        // ── BOOKS ────────────────────────────────────────────────
        public async Task<List<Book>> GetTrendingBooksAsync(int count = 20, int page = 1)
        {
            var url = $"https://openlibrary.org/search.json?q=subject:popular&sort=rating&limit={count}&page={page}";
            var json = await _http.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<OpenLibraryResponse>(json, JsonOpts);

            return data?.Docs.Take(count).Select(b => new Book
            {
                Id = Math.Abs(b.Key.GetHashCode()),
                Title = b.Title,
                Author = b.Author_Name?.FirstOrDefault() ?? "Unknown",
                ImageUrl = b.CoverUrl,
                Rating = Math.Round(b.Ratings_Average ?? 0, 1)
            }).ToList() ?? new();
        }

        public async Task<List<Book>> SearchBooksAsync(string query)
        {
            var url = $"https://openlibrary.org/search.json?q={Uri.EscapeDataString(query)}&limit=20";
            var json = await _http.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<OpenLibraryResponse>(json, JsonOpts);

            return data?.Docs.Take(20).Select(b => new Book
            {
                Id = Math.Abs(b.Key.GetHashCode()),
                Title = b.Title,
                Author = b.Author_Name?.FirstOrDefault() ?? "Unknown",
                ImageUrl = b.CoverUrl,
                Rating = Math.Round(b.Ratings_Average ?? 0, 1)
            }).ToList() ?? new();
        }

        // ── ANIME (AniList GraphQL) ───────────────────────────────
        public async Task<List<Anime>> GetTrendingAnimesAsync(int count = 20, int page = 1)
        {
            var query = "{\"query\": \"query { Page(page: " + page + ", perPage: " + count + ") { media(sort: TRENDING_DESC, type: ANIME, isAdult: false) { id title { romaji } coverImage { large } averageScore episodes status description genres } } }\"}";

            var request = new HttpRequestMessage(HttpMethod.Post, "https://graphql.anilist.co");
            request.Headers.Add("Accept", "application/json");
            request.Content = new StringContent(query, System.Text.Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<AniListResponse>(json, JsonOpts);

            return data?.Data?.Page?.Media?.Select(a => new Anime
            {
                Id = a.Id,
                Title = a.Title?.Romaji ?? "Unknown",
                ImageUrl = a.CoverImage?.Large ?? "/img/no-cover.jpg",
                Rating = Math.Round((a.AverageScore ?? 0) / 10.0, 1),
                Description = StripHtml(a.Description ?? ""),
                Episodes = a.Episodes ?? 0,
                Status = a.Status ?? "",
                Genre = a.Genres?.FirstOrDefault() ?? ""
            }).ToList() ?? new();
        }

        public async Task<Anime?> GetAnimeByIdAsync(int id)
        {
            var query = "{\"query\": \"query { Media(id: " + id + ", type: ANIME) { id title { romaji } coverImage { large } averageScore episodes status description genres } }\"}";

            var request = new HttpRequestMessage(HttpMethod.Post, "https://graphql.anilist.co");
            request.Headers.Add("Accept", "application/json");
            request.Content = new StringContent(query, System.Text.Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<AniListSingleResponse>(json, JsonOpts);

            var a = data?.Data?.Media;
            if (a == null) return null;

            return new Anime
            {
                Id = a.Id,
                Title = a.Title?.Romaji ?? "Unknown",
                ImageUrl = a.CoverImage?.Large ?? "/img/no-cover.jpg",
                Rating = Math.Round((a.AverageScore ?? 0) / 10.0, 1),
                Description = StripHtml(a.Description ?? ""),
                Episodes = a.Episodes ?? 0,
                Status = a.Status ?? "",
                Genre = a.Genres?.FirstOrDefault() ?? ""
            };
        }

        public async Task<List<Anime>> SearchAnimesAsync(string query)
        {
            var gqlQuery = "{\"query\": \"query { Page(page: 1, perPage: 20) { media(search: \\\"" + query.Replace("\"", "") + "\\\", type: ANIME, isAdult: false) { id title { romaji } coverImage { large } averageScore episodes status description genres } } }\"}";

            var request = new HttpRequestMessage(HttpMethod.Post, "https://graphql.anilist.co");
            request.Headers.Add("Accept", "application/json");
            request.Content = new StringContent(gqlQuery, System.Text.Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<AniListResponse>(json, JsonOpts);

            return data?.Data?.Page?.Media?.Select(a => new Anime
            {
                Id = a.Id,
                Title = a.Title?.Romaji ?? "Unknown",
                ImageUrl = a.CoverImage?.Large ?? "/img/no-cover.jpg",
                Rating = Math.Round((a.AverageScore ?? 0) / 10.0, 1),
                Description = StripHtml(a.Description ?? ""),
                Episodes = a.Episodes ?? 0,
                Status = a.Status ?? "",
                Genre = a.Genres?.FirstOrDefault() ?? ""
            }).ToList() ?? new();
        }

        // ── MUSIC (iTunes) ────────────────────────────────────────
        public async Task<List<MusicItem>> GetTopTracksAsync(int count = 10)
        {
            var url = $"https://itunes.apple.com/search?term=top+hits+2025&entity=song&limit={count}&sort=popular";
            var json = await _http.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<ItunesResponse>(json, JsonOpts);

            return data?.Results.Select((t, i) => new MusicItem
            {
                Id = i + 1,
                Title = t.TrackName,
                Artist = t.ArtistName,
                ImageUrl = t.LargeArtwork,
                Type = "Single"
            }).ToList() ?? new();
        }

        public async Task<List<MusicItem>> GetTopAlbumsAsync(int count = 10)
        {
            var url = $"https://itunes.apple.com/search?term=top+albums+2025&entity=album&limit={count}&sort=popular";
            var json = await _http.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<ItunesResponse>(json, JsonOpts);

            return data?.Results.Select((t, i) => new MusicItem
            {
                Id = i + 1,
                Title = t.CollectionName,
                Artist = t.ArtistName,
                ImageUrl = t.LargeArtwork,
                Type = "Album"
            }).ToList() ?? new();
        }
    }
}