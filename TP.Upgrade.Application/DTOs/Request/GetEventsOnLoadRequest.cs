using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetEventsOnLoadRequest
    {
        public int StartIndex { get; set; }
        public int ELimit { get; set; }
        public string CountyCode { get; set; }
        [MaxLength(100)]
        public string Point { get; set; }
        public int Radius { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public long CustomerId { get; set; }
    }
}
