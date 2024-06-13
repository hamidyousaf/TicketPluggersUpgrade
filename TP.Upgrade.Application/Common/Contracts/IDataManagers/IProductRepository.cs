using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IDataManagers
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<bool> InsertProduct(Product product, CancellationToken ct = default);
        IQueryable<ProductLite> GetProductsByVendorId(long vendorId);
        IQueryable<ProductLite> GetProductsByEventId(int eventId);
        IQueryable<MessageSessionDto> GetProductsByVendorIdAndSearchText(long userId, string searchText);
        IQueryable<ListingWrapper> GetAllEventListingsForAdmin(GetAllListingRequest request, decimal defaultCommissionRate);
        IQueryable<ProductSale> getUploadedTicketsForEvent(long eventId);
    }
}
