namespace PetArtworksPlatform.Models.ViewModels
{
    public class ExhibitionList
    {
        public IEnumerable<ExhibitionToListDto> Exhibitions { get; set; }

        public bool isAdmin { get; set; }

        public int Page { get; set; }

        public int MaxPage { get; set; }
    }
}