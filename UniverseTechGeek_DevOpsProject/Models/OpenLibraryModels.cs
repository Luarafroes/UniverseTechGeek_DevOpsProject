namespace Universetechgeek.Models.ApiModels
{
    public class OpenLibraryResponse
    {
        public List<OpenLibraryBook> Docs { get; set; } = new();
    }

    public class OpenLibraryBook
    {
        public string Key { get; set; } = "";
        public string Title { get; set; } = "";
        public List<string>? Author_Name { get; set; }
        public int? Cover_I { get; set; }
        public double? Ratings_Average { get; set; }

        public string CoverUrl => Cover_I.HasValue
            ? $"https://covers.openlibrary.org/b/id/{Cover_I}-L.jpg"
            : "/img/no-cover.jpg";
    }
}
