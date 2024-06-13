using Azure.Core;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IDataManagers
{
    public interface ICustomerRepository: IRepository<Customer>
    {
        Task<bool> InsertCustomer(Customer customer, CancellationToken ct = default);
        IQueryable<CustomerLite> GetCustomerByUsername(string authorName);
        IQueryable<long> GetCustomerIdByUserId(string userId);
        IQueryable<GetCustomerProfileByIdDto> GetCustomerProfileById(long customerId);
        Task<bool> IsBillingAddressExist(long customerId);
        Task<bool> IsCustomerExist(long customerId);
        IQueryable<Customer> GetCustomerWithUserByCustomerId(long customerId);
        IQueryable<GetOrdersByCustomerDto> GetOrdersByCustomerId(long customerId);
        IQueryable<Customer> GetCustomerByEmail(string email);
        IQueryable<Customer> GetVendorsInBulk(List<long> ids);
        IQueryable<Customer> GetCustomersInBulk(List<long> ids);
        IQueryable<Customer> GetCustomerWithCurrencyById(long customerId);
    }
}
