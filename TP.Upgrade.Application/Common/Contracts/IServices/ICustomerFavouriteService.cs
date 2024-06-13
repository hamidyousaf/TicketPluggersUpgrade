using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface ICustomerFavouriteService
    {
        Task<ResponseModel> InsertCustomerFavourite(CreateCustomerFavouriteRequest favouriteRequest, CancellationToken ct = default);
        Task<List<CustomerFavouriteLite>> GetCustomerFavouritesAsync(long customerId, CancellationToken ct = default);
        Task<ResponseModel> ChangeNotifyStatus(FavouriteNotifyRequest notifyRequest, CancellationToken ct = default);
        Task<ResponseModel> DeleteCustomerFavourite(DeleteCustomerFavouriteRequest favouriteRequest, CancellationToken ct = default);
        Task<ResponseModel> GetFavourites(GetFavouriteRequest favouriteRequest, CancellationToken ct);
    }
}
