namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetEventsBySearchRequest
    {
        public string SearchText { get; set; } = string.Empty;
        public string Sort { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }
}
