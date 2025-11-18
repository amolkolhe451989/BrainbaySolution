namespace Brainbay.Web.Models
{
    public class CharacterViewModel
    {
        public int Id { get; set; }
        public int ExternalId { get; set; }
        public string Name { get; set; } = "";
        public string Status { get; set; } = "";
        public string Species { get; set; } = "";
        public string Gender { get; set; } = "";
        public string Origin { get; set; } = "";
        public string Location { get; set; } = "";
        public string ImageUrl { get; set; } = "";
    }
}
