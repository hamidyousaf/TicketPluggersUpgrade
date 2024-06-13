namespace TP.Upgrade.Application.DTOs
{
    public class GetEventByTextDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EventStartUTC { get; set; }
        public string? ImageURL { get; set; }
        public string Venue { get; set; }
    }
}
