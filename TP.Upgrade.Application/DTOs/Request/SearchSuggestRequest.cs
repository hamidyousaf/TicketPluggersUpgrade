namespace TP.Upgrade.Application.DTOs.Request
{
    public class SearchSuggestRequest
    {
        public string SearchText { get; set; }
        public string EntityList { get; set; }
        public int PerfRows { get; set; }
        public string Region { get; set; }
        public long CustomerId { get; set; }
        public long CurrencyId { get; set; }
    }
}
