namespace Universetechgeek.Models.ApiModels
{
    public class AniListResponse
    {
        public AniListData? Data { get; set; }
    }

    public class AniListSingleResponse
    {
        public AniListSingleData? Data { get; set; }
    }

    public class AniListData
    {
        public AniListPage? Page { get; set; }
    }

    public class AniListSingleData
    {
        public AniListMedia? Media { get; set; }
    }

    public class AniListPage
    {
        public List<AniListMedia>? Media { get; set; }
    }

    public class AniListMedia
    {
        public int Id { get; set; }
        public AniListTitle? Title { get; set; }
        public AniListCoverImage? CoverImage { get; set; }
        public int? AverageScore { get; set; }
        public int? Episodes { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public List<string>? Genres { get; set; }
    }

    public class AniListTitle
    {
        public string? Romaji { get; set; }
    }

    public class AniListCoverImage
    {
        public string? Large { get; set; }
    }
}
