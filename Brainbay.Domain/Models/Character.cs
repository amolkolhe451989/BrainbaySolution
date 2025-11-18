namespace Brainbay.Domain.Models
{
    public class Character
    {
        public int Id { get; set; }              // Local DB PK
        public int ExternalId { get; set; }      // Rick & Morty API id
        public string Name { get; set; } = "";
        public string Status { get; set; } = ""; // “Alive”, “Dead”, “unknown”
        public string Species { get; set; } = "";
        public string Gender { get; set; } = "";
        public string Origin { get; set; } = ""; // Origin name
        public string Location { get; set; } = ""; // Current location name
        public string ImageUrl { get; set; } = "";
    }
}
