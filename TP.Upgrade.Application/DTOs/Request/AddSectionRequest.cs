using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class AddSectionRequest
    {
        public long VenueId { get; set; }
        public long ZoneId { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; }
    }
}
