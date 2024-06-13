namespace TP.Upgrade.Application.DTOs
{
    public class GetPopularSearchedEventDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EventStartUTC { get; set; }
        public string StartTime { get; set; }
        public bool IsHotEvent { get; set; }
        public decimal? TicketMinPrice { get; set; }
        public int? TicketAvailable { get; set; }
        public int? EventSearchCount { get; set; } = 0;
        public string? ImageURL { get; set; }
    }
}
