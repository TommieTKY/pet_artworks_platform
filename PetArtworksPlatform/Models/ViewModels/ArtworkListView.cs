namespace PetArtworksPlatform.Models.ViewModels
{
    public class ArtworkListView
    {
        public IEnumerable<ArtworkToListDto> Artworks { get; set; }

        public bool isAdmin { get; set; }

        public int Page { get; set; }

        public int MaxPage { get; set; }
    }
}