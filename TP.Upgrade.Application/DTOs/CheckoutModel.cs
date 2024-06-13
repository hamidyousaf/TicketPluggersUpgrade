using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.Upgrade.Application.DTOs
{
    public class CheckoutModel
    {
        public long TicketId { get; set; }
        public string EventName { get; set; }
        public DateTime EventStart { get; set; }
        public string StartTime { get; set; }
        public string VenueName { get; set; }
        public string VenueCity { get; set; }
        public string VenueCountry { get; set; }
        public string currency { get; set; }
        public decimal OrderTax { get; set; }
        public decimal PlatFormFee { get; set; }
        public int TaxType { get; set; }
        public int FeeType { get; set; }
        public decimal OrderPrice { get; set; }
        public string errorMessage { get; set; }
        public int ticketTypeId { get; set; }
        public string sectionName { get; set; }
        public int ticketRow { get; set; }
        public int seatsFrom { get; set; }
        public int seatsTo { get; set; }
    }
}
