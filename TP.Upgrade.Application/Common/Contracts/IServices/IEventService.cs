using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Helpers.Pagination;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface IEventService
    {
        Task<ResponseModel> InsertEvent(CreateEventRequest Event, CancellationToken ct = default);
        Task<Event> GetById(int eventId, CancellationToken ct = default);
        Task<ResponseModel> GetEventStatuses();
        Task<ResponseModel> GetGenresBySegmentId(short segmentId);
        Task<ResponseModel> GetSubGenresByGenreId(short genreId);
        Task<ResponseModel> GetCustomEvents();
        Task<ResponseModel> DeleteCustomEvent(int id);
        Task<ResponseModel> GetEventsByText(string searchText);
        Task<ResponseModel> SearchEvents(SearchEventRequest request);
        Task<ResponseModel> SearchSuggest(SearchSuggestRequest request);
        Task<ResponseModel> GetPopularSearchedEvent(GetPopularSearchedEventRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetEventById(GetEventByIdRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetEventsByVenueId(GetEventsByVenueIdRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetEventsBySearch(GetEventsBySearchRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetEventListings(GetEventListingsRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetRecentHotEvent(GetRecentHotEventRequest request, CancellationToken ct = default);
        Task<ResponseModel> GetFeaturedEvents(GetFeaturedEventRequest request, CancellationToken ct = default);
        Task<EventOnLoadResponse> GetEventsOnLoad(GetEventsOnLoadRequest request, CancellationToken ct = default);
        Task<EventOnLoadNewResponse> GetEventsOnLoadNew(GetEventsOnLoadRequest request, CancellationToken ct = default);
        Task<List<EventLite>> UpcommingEvents(UpcommingEventRequest request, CancellationToken ct = default);
        Task<List<EventLite>> EventsToSellorDashBoard(EventsToSellorDashBoardRequest request, CancellationToken ct = default);
        Task<PaginationModel<GetEventsByVenueIdNewResponse>> GetEventsByVenueIdNew(SearchEventByEntity request, CancellationToken ct = default);
        Task<PaginationModel<GetEventsByVenueIdNewResponse>> GetEventsBySearchNew(SearchEventByEntity request, CancellationToken ct = default);
        Task<List<EventLite>> GetRecentHotEventWithLimit(SearchEventByEntity eventByEntity, CancellationToken ct = default);
    }
}