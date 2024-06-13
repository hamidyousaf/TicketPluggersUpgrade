using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface ISharedService
    {
        Task<bool> IsEventExist(int eventId);
        Task<ResponseModel> UpdateAddress(UpdateAddressRequest addressRequest, CancellationToken ct = default);
        Task<ResponseModel> ChangePassword(ChangePasswordRequest passwordRequest);
        Task<ResponseModel> UpdateUser(UpdateUserRequest userRequest);
    }
}
