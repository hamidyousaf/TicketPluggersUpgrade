using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.Upgrade.Application.DTOs
{
    public class EventProductViewModel<IEntity>
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
    public class ProductSale
    {
        public long Id { get; set; }
        public int ProductTypeId { get; set; }
        public long eventId { get; set; }
        public string Name { get; set; }
        public long VendorId { get; set; }
        public int SoldQuantity { get; set; }
        public int StockQuantity { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public decimal ProceedCost { get; set; }
        public decimal FaceValue { get; set; }
        public bool HasDiscountsApplied { get; set; }
        public string ticketSeperation { get; set; }
        public int ticketRow { get; set; }
        public int seatsFrom { get; set; }
        public int seatsTo { get; set; }
        public int ticketTypeId { get; set; }
        public int sectionId { get; set; }
        public bool Published { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public bool IsInstantDelivery { get; set; }
        public int TicketSpliting { get; set; }
        public string ProofCerificate { get; set; }
        public string Notes { get; set; }
        public string TranferRecipt { get; set; }
        public string TicketLinks { get; set; }
        public Int64 OrderId { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
