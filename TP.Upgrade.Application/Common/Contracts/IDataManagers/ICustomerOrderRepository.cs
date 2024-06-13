using Azure.Core;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IDataManagers
{
    public interface ICustomerOrderRepository : IRepository<CustomerOrder>
    {
        IQueryable<CustomerOrderLite> GetOrdersByVendorId(long vendorId);
        Task<bool> InsertCustomerOrder(CustomerOrder order, CancellationToken ct = default);
        IQueryable<GetOrderByIdDto> GetOrderById(long orderId, long customerId, bool readOnly = true);
        IQueryable<GetOrderByIdDto> GetOrderById(long orderId, bool readOnly = true);
        IQueryable<GetOrderByIdDto> GetSalesOrdersByCustomerId(long customerId, bool readOnly = true);
        IQueryable<GetOrderByIdDto> GetSalesOrderForAdmin(bool readOnly = true);
        IQueryable<GetOrderByIdDto> GetOrdersByCustomerId(long customerid);
        IQueryable<CustomerOrder> GetOrdersByOrderIds(List<long> orderIds);
        IQueryable<CustomerOrder> GetCompletedOrdersByVendorId(long vendorId);
        IQueryable<GetOrderByIdDto> GetOrders();
        IQueryable<MessageSessionDto> GetOrdersByCustomerIdAndSearchText(long userId, string searchText);
        IQueryable<MessageSessionDto> GetOrdersByVendorIdAndSearchText(long userId, string searchText);
        IQueryable<GetOrderByIdDto> GetOrderByPaymentId(string paymentId);
        IQueryable<GetOrdersByTicketTypeIdDto> GetOrdersByTicketTypeId(byte ticketTypeId);
        //IQueryable<GetOrdersPaymentListDto> GetOrdersPaymentList(long vendorId, string searchText);
        IQueryable<ProductSale> getSaledTicketsForEvent(long eventId);
    }
}
