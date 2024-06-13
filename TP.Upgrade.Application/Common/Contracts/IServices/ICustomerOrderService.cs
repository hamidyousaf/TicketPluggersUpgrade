using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface ICustomerOrderService
    {
        Task<List<CustomerOrderLite>> GetOrdersByVendorId(long vendorId, CancellationToken ct = default);
        Task<ResponseModel> PlaceOrder(CheckoutRequest orderRequest, CancellationToken ct = default);
        Task UpdateTicketQuantityByEventId(int eventId, CancellationToken ct = default);
        Task<ResponseModel> GetSalesOrdersByCustomer(GetCustomersSalesOrderRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetCustomersSalesOrder(long customerId, CancellationToken ct = default);
        Task<ResponseModel> GetSalesOrderForAdmin(GetSalesOrderForAdminRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetSalesTicketForAdmin(GetSalesTicketForAdminRequest request, CancellationToken ct = default);
        Task<ResponseModel> ConfirmMutipleOrderByVender(List<long> orderIds, CancellationToken ct = default);
        Task<ResponseModel> ConfirmOrderByVender(long orderId, CancellationToken ct = default);
        Task<ResponseModel> SalesOrderGraph(long vendorId, CancellationToken ct = default);
        Task<ResponseModel> GetOrderCountByStatus(long customerId, CancellationToken ct = default);
        Task<ResponseModel> OrderCountByStatusForSellorDashbord(long customerId, CancellationToken ct = default);
        Task<ResponseModel> SendGetPayRequest(List<long> orderIds, CancellationToken ct = default);
        Task<ResponseModel> ApproveGetPayRequest(List<long> orderId, CancellationToken ct = default);
        Task<ResponseModel> GetOrdersByPaymentStatus(GetOrdersByPaymentStatusRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetOrdersPaymentOfCustomer(long vendorId, CancellationToken ct = default);
        Task<ResponseModel> ApproveShipmentDate(ApproveShipmentDateRequest request, CancellationToken ct = default);
        Task<ResponseModel> ResetShipment(ResetShipmentRequest request, CancellationToken ct = default);
        Task<ResponseModel> UploadShipmentData(UploadShipmentDataRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetListToMessageSession(GetListToMessageSessionRequest request, CancellationToken ct = default);
        Task<ResponseModel> UpdateOrderTicketsDownloaded(long orderId, CancellationToken ct = default);
        Task<ResponseModel> GetOrderDetails(long orderId, CancellationToken ct = default);
        Task<ResponseModel> GetOrderByPaymentId(string paymentId, CancellationToken ct = default);
        Task<ResponseModel> GetAllTimeExtensionOrders(int tab, CancellationToken ct = default);
        //Task<ResponseModel> GetOrdersPaymentList(long vendorId, string searchText, CancellationToken ct = default);
    }
}
