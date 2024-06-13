namespace TP.Upgrade.Application.DTOs.Request
{
    public class EventsToSellorDashBoardRequest
    {
        public int page { get; set; }
        public int limit { get; set; }
        public string Point { get; set; }
        public string SearchBy { get; set; }
        public string CountyCode { get; set; }
        public int Genre { get; set; }
        public int RadiusFrom { get; set; }
        public int RadiusTo { get; set; }
        public int Filter { get; set; }
        public string CountryCode { get; set; }
    }
}
