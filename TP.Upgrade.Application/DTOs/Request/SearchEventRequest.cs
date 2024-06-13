namespace TP.Upgrade.Application.DTOs.Request
{
    public class SearchEventRequest
    {
        public int EventId { get; set; } = 0;
        public string SearchText { get; set; }
        public short CategoryId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }
}
