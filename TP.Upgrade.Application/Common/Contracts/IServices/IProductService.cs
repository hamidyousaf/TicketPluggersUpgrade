using Azure.Core;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface IProductService
    {
        Task<ResponseModel> InsertProduct(CreateProductRequest product, CancellationToken ct = default);
        Task<List<ProductLite>> GetProductsByVendorId(long vendorId, CancellationToken ct = default);
        Task<ResponseModel> GetVendorAllProductSummary(GetVendorAllProductSummaryRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetVendorProductByEventId(long eventId, long vendorId, CancellationToken ct = default);
        Task<ResponseModel> GetProductByIdForEdit(long productId, CancellationToken ct = default);
        Task<ResponseModel> GetProductByIdForEditForApp(long productId, CancellationToken ct = default);
        Task<ResponseModel> InsertMultipleProduct(List<CreateProductRequest> request, long vendorId, CancellationToken ct = default);
        Task<ResponseModel> GetCustomerListingById(int productId, CancellationToken ct = default);
        Task<ResponseModel> DeleteProduct(List<int> productIds, CancellationToken ct = default);
        Task<ResponseModel> TicketListingForVendor(TicketListingForVendorRequest request, CancellationToken ct = default);
        Task<ResponseModel> TicketMarketAnalysis(TicketMarketAnalysisRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetTicketDocument(long orderId, CancellationToken ct = default);
        Task<ResponseModel> UpdatePublishStatus(List<UpdatePublishStatusRequest> request, CancellationToken ct = default);
        Task<ResponseModel> GetAllEventListingsForAdmin(GetAllListingRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetCustomerListings(GetAllListingRequest request, CancellationToken ct = default);
        Task<ResponseModel> UpdateProduct(UpdateProductRequest request, CancellationToken ct = default);
        Task<ResponseModel> PublishListingOfEvent(PublishListingOfEventRequest request, long vendorId , CancellationToken ct = default);
        Task<ResponseModel> EditProductType(EditProductTypeRequest request, CancellationToken ct = default);
        Task<ResponseModel> UpdateProductShotInfo(UpdateProductShotInfoRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetListingPriceWithCheck(GetListingPriceWithCheckRequest request, CancellationToken ct = default);
        Task<DownloadTicketDto> DownloadTicket(long orderId, long documentId, CancellationToken ct = default);
        Task<List<ListingWrapper>> GetTicketListingsForCustomer(long Id, CancellationToken ct = default);
    }
}
