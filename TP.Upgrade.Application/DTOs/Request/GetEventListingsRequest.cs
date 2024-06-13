namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetEventListingsRequest
    {
        public int EventId { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }
}
