using System.Threading;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface IAddressService
    {
        Task<ResponseModel> InsertAddress(CreateAddressRequest addressRequest, CancellationToken ct = default);
        Task<ResponseModel> UpdateAddress(UpdateAddressRequest addressRequest, CancellationToken ct = default);
        Task<ResponseModel> DeleteAddress(DeleteAddressRequest addressRequest, CancellationToken ct = default);
        Task<ResponseModel> GetAddressesByCustomerId(long customerId);
    }
}
