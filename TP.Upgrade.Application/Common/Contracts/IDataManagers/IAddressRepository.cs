using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IDataManagers
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<bool> InsertAddress(Address address, CancellationToken ct = default);
        IQueryable<AddressLite> GetAddressesByCustomerId(long customerId);
        IQueryable<AddressLite> GetAddressByIdAndCustomerId(int addressId, long customerId);
    }
}
