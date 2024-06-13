using Microsoft.AspNetCore.Http;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class ReportProblemRequest
    {
        public long? OrderId { get; set; }
        public int? ProductId { get; set; }
        public IFormFile File { get; set; }
        public string? FileUrl { get; set; }
        public string ReferenceLink { get; set; }
        public string Message { get; set; }
        public long IssueType { get; set; }
        public int? EventId { get; set; }
        public long CustomerId { get; set; } = 0;
    }
}
