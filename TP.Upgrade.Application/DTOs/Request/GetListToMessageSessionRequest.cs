using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetListToMessageSessionRequest
    {
        public int Tab { get; set; } = 1;
        public long UserId { get;set;}
        public string SearchText { get;set;} = string.Empty;
    }
}
