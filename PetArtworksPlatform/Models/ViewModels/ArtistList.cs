namespace PetArtworksPlatform.Models.ViewModels
{
    public class ArtistList
    {
        // This ViewModel is the structure needed for us to render
        // ArtistPage/List.cshtml

        public IEnumerable<ArtistToListDto> Artists { get; set; }

        public bool isAdmin { get; set; }

        public int Page { get; set; }

        public int MaxPage { get; set; }
    }
}
