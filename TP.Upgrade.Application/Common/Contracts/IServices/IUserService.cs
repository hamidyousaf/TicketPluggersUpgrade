using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterRequest request);

        Task<UserManagerResponse> LoginUserAsync(LoginRequest request);

        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);

        Task<UserManagerResponse> ForgetPasswordAsync(string email);

        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
