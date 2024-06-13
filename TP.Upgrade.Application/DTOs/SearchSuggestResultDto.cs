namespace TP.Upgrade.Application.DTOs
{
    public class SearchSuggestResultDto
    {
        public IList<EventLite> Events { get; set; }
        public IList<VenueLite> Venues { get; set; }
    }
}
