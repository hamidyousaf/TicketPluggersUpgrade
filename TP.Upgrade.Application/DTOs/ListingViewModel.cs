using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.Upgrade.Domain.Helpers.Pagination;

namespace TP.Upgrade.Application.DTOs
{
    public class ListingViewModel
    {
        public ListingViewModel()
        {
            PagingFilteringContext = new PagingFilteringModel();
        }

        public PagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ListingWrapper> Listings { get; set; }

    }
}
