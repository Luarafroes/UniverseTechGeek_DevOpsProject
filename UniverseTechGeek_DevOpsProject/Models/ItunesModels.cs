namespace Universetechgeek.Models.ApiModels
{
    public class ItunesResponse
    {
        public List<ItunesItem> Results { get; set; } = new();
    }

    public class ItunesItem
    {
        public string TrackName { get; set; } = "";
        public string CollectionName { get; set; } = "";
        public string ArtistName { get; set; } = "";
        public string ArtworkUrl100 { get; set; } = "";
        public string LargeArtwork => ArtworkUrl100.Replace("100x100", "600x600");
    }
}
