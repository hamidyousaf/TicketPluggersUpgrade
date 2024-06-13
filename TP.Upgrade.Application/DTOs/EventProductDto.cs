using System.ComponentModel.DataAnnotations.Schema;

namespace TP.Upgrade.Application.DTOs
{
    public class EventProductDto<IEntity>
    {
        public Int64 Id { get; set; }
        public Int64 VeneuId { get; set; }
        public Int64 VendorId { get; set; }
        public string EventName { get; set; }
        public string VenueName { get; set; }
        public string VenueCountry { get; set; }
        public string VenueCity { get; set; }
        public string Venue { get; set; }
        public DateTime EventDate { get; set; }
        public string StartTime { get; set; }
        public string VendorName { get; set; }
        public String EventStartTime { get; set; }
        public int VenderSoldTicketCount { get; set; }
        public int? AvailableTicket { get; set; }
        public int SoldTickects { get; set; }
        public int EventStatus { get; set; }
        [NotMapped]
        public bool IsExpired { get; set; }
        public TimeSpan HoursPending { get; set; }
        public DateTime OnSaleEndDateTime { get; set; }
        //public double FullfilmentDays { get; set; }
        public IList<IEntity> Tickets { get; set; }
    }
}
