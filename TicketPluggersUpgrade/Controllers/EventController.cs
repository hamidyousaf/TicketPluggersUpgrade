using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs.Request;

namespace TP.Upgrade.Api.Controllers
{
    public class EventController : ApiBaseController
    {
        private readonly IEventService _eventService;
        private readonly ISearchKeyService _searchKeyService;
        private readonly ICategoryService _categoryService;
        public EventController(
            IEventService eventService,
            ISearchKeyService searchKeyService,
            ICategoryService categoryService)
        {
            _eventService = eventService;
            _searchKeyService = searchKeyService;
            _categoryService = categoryService;
        }
        [AllowAnonymous, HttpPost("CreateEvent")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> InsertEvent([FromForm] CreateEventRequest model)
        {
            var result = await _eventService.InsertEvent(model);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetEventStatuses")]
        public async Task<IActionResult> GetEventStatuses()
        {
            var result = await _eventService.GetEventStatuses();
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetGenresBySegmentId/{segmentId}")]
        public async Task<IActionResult> GetGenresBySegmentId(short segmentId)
        {
            var result = await _eventService.GetGenresBySegmentId(segmentId);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetSubGenresByGenreId/{genreId}")]
        public async Task<IActionResult> GetSubGenresByGenreId(short genreId)
        {
            var result = await _eventService.GetSubGenresByGenreId(genreId);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetCustomEvents")]
        public async Task<IActionResult> GetCustomEvents()
        {
            var result = await _eventService.GetCustomEvents();
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("DeleteCustomEvent/{id}")]
        public async Task<IActionResult> DeleteCustomEvent(int id)
        {
            var result = await _eventService.DeleteCustomEvent(id);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("SearchByText")]
        public async Task<IActionResult> GetEventsByText(string searchText)
        {
            var result = await _eventService.GetEventsByText(searchText);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("SearchEventForClientSell")]
        public async Task<IActionResult> SearchEventForClientSell(SearchEventRequest request)
        {
            var result = await _eventService.SearchEvents(request);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("SearchByMultipleParam")]
        public async Task<IActionResult> SearchByMultipleParam(SearchEventRequest request)
        {
            var result = await _eventService.SearchEvents(request);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("SearchSuggest")]
        public async Task<IActionResult> SearchSuggest([FromQuery] SearchSuggestRequest request)
        {
            var result = await _eventService.SearchSuggest(request);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetPopularSerchedEvents")]
        public async Task<IActionResult> GetPopularSearchedEvent([FromQuery] GetPopularSearchedEventRequest request, CancellationToken ct)
        {
            var result = await _eventService.GetPopularSearchedEvent(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetEventSearchKey")]
        public async Task<IActionResult> GetTopSearchKeyList([FromQuery] GetTopSearchKeyListRequest request, CancellationToken ct)
        {
            var result = await _searchKeyService.GetTopSearchKeyList(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories(CancellationToken ct)
        {
            var result = await _categoryService.GetCategories(ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("getEventById")]
        public async Task<IActionResult> GetEventById([FromQuery] GetEventByIdRequest request, CancellationToken ct)
        {
            var result = await _eventService.GetEventById(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("geteventsbyvenueId")]
        public async Task<IActionResult> GetEventsByVenueId(GetEventsByVenueIdRequest request, CancellationToken ct)
        {
            var result = await _eventService.GetEventsByVenueId(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("geteventsbySearch")]
        public async Task<IActionResult> GetEventsBySearch(GetEventsBySearchRequest request, CancellationToken ct)
        {
            var result = await _eventService.GetEventsBySearch(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("getEventListings")]
        public async Task<IActionResult> GetEventListings(GetEventListingsRequest request, CancellationToken ct)
        {
            var result = await _eventService.GetEventListings(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetRecentHotEvent")]
        public async Task<IActionResult> GetRecentHotEvent([FromQuery] GetRecentHotEventRequest request, CancellationToken ct)
        {
            var result = await _eventService.GetRecentHotEvent(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet("GetFeaturedEvents")]
        public async Task<IActionResult> GetFeaturedEvents([FromQuery] GetFeaturedEventRequest request, CancellationToken ct)
        {
            var result = await _eventService.GetFeaturedEvents(request, ct);
            return Ok(result);
        }
        //[HttpPost("GetRecentHotEventWithLimit")]
        //public async Task<IActionResult> GetRecentHotEventWithLimit(SearchEventByEntity eventByEntity)
        //{
        //    var result = _eventService.GetRecentHotEventWithFilter(eventByEntity);
        //    return Ok(new ApiOkResponse(result));
        //}
            // {
            //  "startIndex": 0,
            //  "eLimit": 12,
            //  "countyCode": "GB",
            //  "point": "51.498255,-0.115023",
            //  "radius": 1000,
            //  "categoryId": 239,
            //  "subCategoryId": 239,
            //  "eventStartDate": "2022-05-27",
            //  "customerId": 5
            //}
    [AllowAnonymous, HttpPost("geteventsonload")]
        public async Task<IActionResult> GetEventsOnLoad(GetEventsOnLoadRequest request, CancellationToken ct)
        {
            var result = await _eventService.GetEventsOnLoad(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("add-event-poster")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddPoster([FromForm] AddPosterRequest request, CancellationToken ct)
        {
            var result = await _categoryService.AddPoster(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("geteventsListingonload")]
        public async Task<IActionResult> GetEventsOnLoadNew(GetEventsOnLoadRequest request, CancellationToken ct)
        {
            var result = await _eventService.GetEventsOnLoadNew(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("UpcommingEvents")]
        public async Task<IActionResult> UpcommingEvents(UpcommingEventRequest request, CancellationToken ct)
        {
            var result = await _eventService.UpcommingEvents(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("EventsToSellorDashBoard")]
        public async Task<IActionResult> EventsToSellorDashBoard(EventsToSellorDashBoardRequest request, CancellationToken ct)
        {
            var result = await _eventService.EventsToSellorDashBoard(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("geteventsbyvenueIdNew")]
        public async Task<IActionResult> GetEventsByVenueIdNew(SearchEventByEntity request, CancellationToken ct)
        {
            var result = await _eventService.GetEventsByVenueIdNew(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("geteventsbySearchNew")]
        public async Task<IActionResult> GetEventsBySearchNew(SearchEventByEntity request, CancellationToken ct)
        {
            var result = await _eventService.GetEventsBySearchNew(request, ct);
            return Ok(result);
        }
        [AllowAnonymous, HttpPost("GetRecentHotEventWithLimit")]
        public async Task<IActionResult> GetRecentHotEventWithLimit(SearchEventByEntity eventByEntity, CancellationToken ct)
        {
            var result = await _eventService.GetRecentHotEventWithLimit(eventByEntity);
            return Ok(result);
        }
    }
}
// pending API's
///api/Event/GetEventListingsWithFilter