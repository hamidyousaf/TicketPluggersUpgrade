using Azure.Core;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IDataManagers
{
    public interface ICustomerFavouriteRepository : IRepository<CustomerFavourite>
    {
        Task<bool> InsertCustomerFavourite(CustomerFavourite customerFavourite, CancellationToken ct = default);
        IQueryable<CustomerFavouriteLite> GetCustomerFavourites(long customerId);
        IQueryable<CustomerFavouriteLite> GetCustomerFavouritesByEventId(long customerId, int eventId);
        IQueryable<CustomerFavouriteLite> GetCustomerFavouritesByVenueId(long customerId, int venueId);
        IQueryable<CustomerFavouriteLite> GetCustomerFavouritesByPerformerId(long customerId, int performerId);
        IQueryable<CustomerFavourite> GetCustomerFavouriteById(long customerId, int favouriteId);
        IQueryable<GetFavouriteEventDto> GetCustomerFavouriteEventsBySearch(long customerId, string searchText);
        IQueryable<GetFavouriteVenueDto> GetCustomerFavouriteVenuesBySearch(long customerId, string searchText);
        Task<bool> IsFavouriteEventByCustomerId(long customerId, long eventId);
        IQueryable<CustomerFavouriteLite> GetFavouriteEventsByCustomerId(long customerId);
    }
}
