using Azure.Core;
using Microsoft.AspNet.SignalR;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IDataManagers
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<bool> InsertEvent(Event Event, CancellationToken ct = default);
        IQueryable<GetCustomEventDto> GetCustomEvents();
        IQueryable<GetEventByTextDto> GetEventByText(string searchText);
        IQueryable<SearchEventDto> SearchEvents(string searchText, DateTime? startDate, DateTime? endDate, long eventId, int categoryId = 0);
        IQueryable<EventLite> SearchSuggestEvents(string searchText);
        IQueryable<GetEventsByVenueIdDto> GetEventsByVenueId(long venueId);
        IQueryable<GetEventsBySearchDto> GetEventsBySearch(string searchText);
        Task<bool> IsEventExist(int eventId);
        IQueryable<GetFavouriteEventDto> GetCustomerFavouriteEventsBySearchAndEventIds(string searchText, int[] eventIds);
        IQueryable<GetPopularSearchedEventDto> GetPopularSearchedEvent(GetPopularSearchedEventRequest request);
        IQueryable<EventLite> GetEventById(long eventId);
        IQueryable<EventLite> UpcomminEvents();
        IQueryable<EventLite> GetEventByIds(List<int> eventIds);
        IQueryable<EventLite> GetTopRecentHotEventByCountryCode(string countryCode);
        IQueryable<EventLite> GetFeaturedEventsBySearchAndCountryCode(string searchText, string countryCode = "GB");
        List<EventOnLoadViewModel> GetEventsOnLoad(GetEventsOnLoadRequest request);
        List<EventOnLoadViewModel> GetEventsOnLoadNew(GetEventsOnLoadRequest request);
        List<EventLite> FilterEvents(SearchEventByEntity eventSearch);
        List<GetEventsByVenueIdNewResponse> GetEventsByVenueIdNew(SearchEventByEntity request); 
        List<GetEventsByVenueIdNewResponse> GetEventsBySearchNew(SearchEventByEntity request); 
        List<EventLite> GetHotEvent(SearchEventByEntity eventSearch);
        EventProductViewModel<ProductSale> GetEventInfo(long eventId);
    }
}
