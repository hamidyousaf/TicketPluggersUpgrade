using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class ReportProblem : BaseEntity<long>
    {
        public long CustomerId { get; set; }
        public bool IsCustomerSend { get; set; }    // Is message send by customer or admin side.
        public int? EventId { get; set; }
        public long? OrderId { get; set; }
        public int? ProductId { get; set; }
        public string? PaymentId { get; set; }
        public byte ChatType { get; set; }          // Enquiry, Refund, ReportProblem, GeneralEnquery
        public string Message { get; set; }
        public string ReferenceLink { get; set; }
        public string ReferenceFile { get; set; }
        public long IssueType { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        // navigational property.
        public Customer? Customer { get; set; }
        public Event? Event { get; set; }
        public CustomerOrder? Order { get; set; }
        public Product? Product { get; set; }
    }
}