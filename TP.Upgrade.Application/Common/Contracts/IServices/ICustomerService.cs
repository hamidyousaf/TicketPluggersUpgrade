using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface ICustomerService
    {
        Task<ResponseModel> InsertCustomer(CreateCustomerRequest customer, CancellationToken ct = default);
        Task<Customer> GetById(long customerId, CancellationToken ct = default);
        Task<ResponseModel> GetCustomerByUsername(string authorName);
        Task<long> GetCustomerIdByUserId(string userId);
        Task<ResponseModel> GetCustomerProfileById(long customerId, CancellationToken ct = default);
        Task UpdateBillingAddress(long customerId,int addressId);
        Task<bool> IsBillingAddressExist(long customerId);
        Task<ResponseModel> UpdateCustomer(UpdateCustomerRequest customerProfile, CancellationToken ct = default);
        Task<ResponseModel> GetCustomerSimpleProfile(long customerId, CancellationToken ct = default);
        Task<ResponseModel> GetOrdersByCustomer(GetOrdersByCustomerRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetOrderById(long orderId, long customerId, CancellationToken ct = default);
        Task<ResponseModel> GetOrderById(long orderId, CancellationToken ct = default);
        Task<ResponseModel> EmployeeRegister(EmployeeRegisterRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetOrdersByCustomerId(int customerid, CancellationToken ct = default);
        Task<ResponseModel> ChangeVendorAccountStatusInBulk(List<long> ids, CancellationToken ct = default);
        Task<ResponseModel> ChangeCustomerAccountStatusInBulk(List<long> ids, CancellationToken ct = default);
        Task<ResponseModel> ChangeCustomerCurrency(long customerId , short currencyId, CancellationToken ct = default);
        Task<ResponseModel> DeleteCustomersInBulk(List<long> customerIds, CancellationToken ct = default);
    }
}
