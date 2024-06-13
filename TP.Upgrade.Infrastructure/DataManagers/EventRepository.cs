using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class EventRepository : Repository<Event>, IEventRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public EventRepository(
            TP_DbContext _dbContext,
            IMapper mapper,
            IConfiguration configuration) : base(_dbContext)
        {
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<bool> InsertEvent(Event Event, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return false;

            var result = await Add(Event);

            if (result is 0)
            {
                return false;
            }

            return true;
        }
        public IQueryable<GetCustomEventDto> GetCustomEvents()
        {
            return GetAll()
                .Include(x => x.Venue)
                .Where(x => x.IsCustom == true)
                .ProjectTo<GetCustomEventDto>(_mapper.ConfigurationProvider)
                .AsQueryable();
        }

        public IQueryable<GetEventByTextDto> GetEventByText(string searchText)
        {
            return GetAll()
                .Include(x => x.Venue)
                .Where(x => x.Name.ToLower().Contains(searchText.ToLower()) && x.EventStartUTC > DateTime.UtcNow)
                .ProjectTo<GetEventByTextDto>(_mapper.ConfigurationProvider)
                .AsQueryable();
        }
        public IQueryable<SearchEventDto> SearchEvents(string searchText, DateTime? startDate, DateTime? endDate, long eventId, int categoryId = 0)
        {
            var query = GetAll()
                .Include(x => x.Venue)
                .Where(x => ((categoryId > 0 && (categoryId == x.SegmentId || categoryId == x.SubGenreId)) || categoryId == 0) && x.EventStartUTC > DateTime.UtcNow)
                .ProjectTo<SearchEventDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            var text = searchText.ToLower();
            // get event
            if (eventId > 0)
                query = query.Where(x => x.Id == eventId);

            // search by text
            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(x => x.Name.ToLower().Contains(text) || x.Venue.ToLower().Contains(text) ||
                                x.City.ToLower().Contains(text));

            // get by date
            if (startDate.HasValue)
                query = query.Where(x => x.EventStartUTC >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(x => endDate.Value >= x.EventStartUTC);

            // set order
            query = query.OrderBy(x => x.EventStartUTC).OrderByDescending(x => x.TicketAvailable);
            return query;
        }
        public IQueryable<EventLite> SearchSuggestEvents(string searchText)
        {
            searchText = searchText.Replace(" ", "");
            var query = GetAll()
                .Include(x => x.Venue)
                .ProjectTo<EventLite>(_mapper.ConfigurationProvider)
                .Where(x => x.EventStartUTC > DateTime.UtcNow);

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query
                    .Where(c => c.SearchName.Contains(searchText) ||
                        c.Venue.ToLower().Contains(searchText) ||
                        c.City.ToLower().Contains(searchText))
                    .OrderBy(p => p.TicketAvailable)
                    .OrderBy(x => x.EventStartUTC);

            return query;
        }
        public IQueryable<GetEventsByVenueIdDto> GetEventsByVenueId(long venueId)
        {
            return GetReadOnlyList()
                .Include(x => x.Venue)
                .Where(x => x.VenueId == venueId)
                .ProjectTo<GetEventsByVenueIdDto>(_mapper.ConfigurationProvider);
        }

        public async Task<bool> IsEventExist(int eventId)
        {
            return await GetAll().Select(x => x.Id).AnyAsync(x => x == eventId);
        }

        public IQueryable<GetFavouriteEventDto> GetCustomerFavouriteEventsBySearchAndEventIds(string searchText, int[] eventIds)
        {
            return GetAll()
                .Include(x => x.Venue)
                .Where(x => x.Name.ToLower().Contains(searchText.ToLower()) && eventIds.Contains(x.Id))
                .ProjectTo<GetFavouriteEventDto>(_mapper.ConfigurationProvider);
        }

        public IQueryable<GetPopularSearchedEventDto> GetPopularSearchedEvent(GetPopularSearchedEventRequest request)
        {
            var skip = (request.Page - 1) * request.Limit;
            return GetReadOnlyList()
                .ProjectTo<GetPopularSearchedEventDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(x => x.EventSearchCount)
                .Skip(skip)
                .Take(request.Limit);
        }

        public IQueryable<EventLite> GetEventById(long eventId)
        {
            return GetReadOnlyList()
                .Where(x => x.Id == eventId)
                .ProjectTo<EventLite>(_mapper.ConfigurationProvider);
        }
        public IQueryable<EventLite> UpcomminEvents()
        {
            var todate = DateTime.UtcNow.AddDays(30);
            return GetReadOnlyList()
                .Include(x => x.Venue)
                .Where(x => x.EventStartUTC > DateTime.UtcNow && x.EventStartUTC < todate && x.Published)
                .ProjectTo<EventLite>(_mapper.ConfigurationProvider);
        }
        public IQueryable<EventLite> GetEventByIds(List<int> eventIds)
        {
            return GetReadOnlyList()
                .Where(x => eventIds.Contains(x.Id))
                .ProjectTo<EventLite>(_mapper.ConfigurationProvider);
        }

        public IQueryable<GetEventsBySearchDto> GetEventsBySearch(string searchText)
        {
            return GetReadOnlyList()
                .Include(x => x.Venue)
                .Where(x => x.Name.ToLower().Contains(searchText.ToLower()) || x.Venue.VenueName.ToLower().Contains(searchText.ToLower()))
                .ProjectTo<GetEventsBySearchDto>(_mapper.ConfigurationProvider);
        }

        public IQueryable<EventLite> GetTopRecentHotEventByCountryCode(string countryCode)
        {
            return GetReadOnlyList()
                .Include(x => x.Venue)
                .Where(x => x.Venue.CountryCode.ToLower().Contains(countryCode.ToLower()) && x.IsHotEvent && x.EventStartUTC > DateTime.UtcNow)
                .OrderBy(x => x.EventStartUTC)
                .Take(10)
                .ProjectTo<EventLite>(_mapper.ConfigurationProvider);
        }

        public IQueryable<EventLite> GetFeaturedEventsBySearchAndCountryCode(string searchText, string countryCode = "GB")
        {
            return GetReadOnlyList()
                .Include(x => x.Venue)
                .Where(x => x.Name.ToUpper().Contains(searchText.ToUpper()) && x.Venue.CountryCode.ToLower().Contains(countryCode.ToLower()) && x.IsFeatured == true && x.EventStartUTC > DateTime.UtcNow)
                .OrderBy(x => x.EventStartUTC)
                .ProjectTo<EventLite>(_mapper.ConfigurationProvider);
        }

        public List<EventOnLoadViewModel> GetEventsOnLoad(GetEventsOnLoadRequest request)
        {

            var radiusfrom = 0;
            List<EventOnLoadViewModel> eventlist = new List<EventOnLoadViewModel>();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:TPConnection"]))
            {
                var pointLatLong = request.Point.Split(",");
                SqlCommand command = new SqlCommand("GetEventsOnLoad", connection);
                command.CommandTimeout = 400;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@startIndex", request.StartIndex));
                command.Parameters.Add(new SqlParameter("@elimit", request.ELimit));
                command.Parameters.Add(new SqlParameter("@point", request.Point));
                command.Parameters.Add(new SqlParameter("@latitude", pointLatLong.Length > 0 ? pointLatLong[0] : ""));
                command.Parameters.Add(new SqlParameter("@longitude", pointLatLong.Length > 1 ? pointLatLong[1] : ""));
                command.Parameters.Add(new SqlParameter("@radiusFrom", radiusfrom));
                command.Parameters.Add(new SqlParameter("@radiusTo", request.Radius));
                command.Parameters.Add(new SqlParameter("@categoryId", request.CategoryId));
                command.Parameters.Add(new SqlParameter("@subCategoryId", request.SubCategoryId));
                command.Parameters.Add(new SqlParameter("@eventStartDate", request.EventStartDate));
                command.Parameters.Add(new SqlParameter("@eventEndDate", request.EventEndDate));
                command.Parameters.Add(new SqlParameter("@countryCode", request.CountyCode));

                connection.Open();
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        EventOnLoadViewModel eventOnLoad = new EventOnLoadViewModel
                        {
                            Event_ID = reader.IsDBNull("Event_ID") ? "" : Convert.ToString(reader["Event_ID"]),
                            Id = reader.IsDBNull("Id") ? 0 : Convert.ToInt32(reader["Id"]),
                            Name = reader.IsDBNull("Name") ? "" : Convert.ToString(reader["Name"]),
                            Description = reader.IsDBNull("Description") ? "" : Convert.ToString(reader["Description"]),
                            Notes = reader.IsDBNull("Notes") ? "" : Convert.ToString(reader["Notes"]),
                            EventStatusId = reader.IsDBNull("EventStatusId") ? 0 : Convert.ToInt32(reader["EventStatusId"]),
                            EventStartUTC = reader.IsDBNull("EventStartUTC") ? DateTime.MinValue : Convert.ToDateTime(reader["EventStartUTC"]),
                            EventEndUTC = reader.IsDBNull("EventEndUTC") ? DateTime.MinValue : Convert.ToDateTime(reader["EventEndUTC"]),
                            EventStart = reader.IsDBNull("EventStartUTC") ? DateTime.MinValue : Convert.ToDateTime(reader["EventStartUTC"]),
                            EventStartTime = reader.IsDBNull("EventStartTime") ? TimeSpan.Zero : TimeSpan.Parse(Convert.ToString(reader["EventStartTime"])),
                            ImageURL = reader.IsDBNull("ImageURL") ? "" : Convert.ToString(reader["ImageURL"]),
                            IsHotEvent = reader.IsDBNull("IsHotEvent") ? false : Convert.ToBoolean(reader["IsHotEvent"]),
                            Published = reader.IsDBNull("Published") ? false : Convert.ToBoolean(reader["Published"]),
                            Deleted = reader.IsDBNull("IsDeleted") ? false : Convert.ToBoolean(reader["IsDeleted"]),
                            Country = reader.IsDBNull("CountryCode") ? "" : Convert.ToString(reader["CountryCode"]),
                            MinPrice = reader.IsDBNull("TicketMinPrice") ? 0 : Convert.ToDecimal(reader["TicketMinPrice"]),
                            MaxPrice = reader.IsDBNull("TicketMaxPrice") ? 0 : Convert.ToDecimal(reader["TicketMaxPrice"]),
                            AvailableTicket = reader.IsDBNull("AvailableTickets") ? 0 : Convert.ToInt32(reader["AvailableTickets"]),
                            SegmentId = reader.IsDBNull("SegmentId") ? 0 : Convert.ToInt32(reader["SegmentId"]),
                            VenueID = reader.IsDBNull("VenueID") ? 0 : Convert.ToInt32(reader["VenueID"]),
                        };
                        eventlist.Add(eventOnLoad);
                    }
                }
            }
            return eventlist;
        }
        public List<EventOnLoadViewModel> GetEventsOnLoadNew(GetEventsOnLoadRequest request)
        {

            var radiusfrom = 0;
            List<EventOnLoadViewModel> eventlist = new List<EventOnLoadViewModel>();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:TPConnection"]))
            {
                var pointLatLong = request.Point.Split(",");
                SqlCommand command = new SqlCommand("GetEventsOnLoadNew", connection);
                command.CommandTimeout = 400;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@startIndex", request.StartIndex));
                command.Parameters.Add(new SqlParameter("@elimit", request.ELimit));
                // command.Parameters.Add(new SqlParameter("@point", eventSearch.Point));
                command.Parameters.Add(new SqlParameter("@latitude", pointLatLong.Length > 0 ? pointLatLong[0] : ""));
                command.Parameters.Add(new SqlParameter("@longitude", pointLatLong.Length > 1 ? pointLatLong[1] : ""));
                //command.Parameters.Add(new SqlParameter("@radius", eventSearch.Radius));
                command.Parameters.Add(new SqlParameter("@radiusFrom", radiusfrom));
                command.Parameters.Add(new SqlParameter("@radiusTo", request.Radius));
                command.Parameters.Add(new SqlParameter("@categoryId", request.CategoryId));
                command.Parameters.Add(new SqlParameter("@subCategoryId", request.SubCategoryId));
                command.Parameters.Add(new SqlParameter("@eventStartDate", request.EventStartDate));
                command.Parameters.Add(new SqlParameter("@eventEndDate", request.EventEndDate));
                command.Parameters.Add(new SqlParameter("@skip", request.ELimit / 2));
                command.Parameters.Add(new SqlParameter("@countryCode", request.CountyCode));

                connection.Open();
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        EventOnLoadViewModel eventOnLoad = new EventOnLoadViewModel
                        {
                            Event_ID = reader.IsDBNull("Event_ID") ? "" : Convert.ToString(reader["Event_ID"]),
                            Id = reader.IsDBNull("Id") ? 0 : Convert.ToInt32(reader["Id"]),
                            Name = reader.IsDBNull("Name") ? "" : Convert.ToString(reader["Name"]),
                            Description = reader.IsDBNull("Description") ? "" : Convert.ToString(reader["Description"]),
                            Notes = reader.IsDBNull("Notes") ? "" : Convert.ToString(reader["Notes"]),
                            EventStatusId = reader.IsDBNull("EventStatusId") ? 0 : Convert.ToInt32(reader["EventStatusId"]),
                            EventStartUTC = reader.IsDBNull("EventStartUTC") ? DateTime.MinValue : Convert.ToDateTime(reader["EventStartUTC"]),
                            EventEndUTC = reader.IsDBNull("EventEndUTC") ? DateTime.MinValue : Convert.ToDateTime(reader["EventEndUTC"]),
                            EventStart = reader.IsDBNull("EventStartUTC") ? DateTime.MinValue : Convert.ToDateTime(reader["EventStartUTC"]),
                            EventStartTime = reader.IsDBNull("EventStartTime") ? TimeSpan.Zero : TimeSpan.Parse(Convert.ToString(reader["EventStartTime"])),
                            ImageURL = reader.IsDBNull("ImageURL") ? "" : Convert.ToString(reader["ImageURL"]),
                            IsHotEvent = reader.IsDBNull("IsHotEvent") ? false : Convert.ToBoolean(reader["IsHotEvent"]),
                            Published = reader.IsDBNull("Published") ? false : Convert.ToBoolean(reader["Published"]),
                            Deleted = reader.IsDBNull("IsDeleted") ? false : Convert.ToBoolean(reader["IsDeleted"]),
                            MinPrice = reader.IsDBNull("TicketMinPrice") ? 0 : Convert.ToDecimal(reader["TicketMinPrice"]),
                            AvailableTicket = reader.IsDBNull("AvailableTickets") ? 0 : Convert.ToInt32(reader["AvailableTickets"]),
                            VenueID = reader.IsDBNull("VenueID") ? 0 : Convert.ToInt32(reader["VenueID"]),
                        };
                        eventlist.Add(eventOnLoad);
                    }
                }
            }
            return eventlist;
        }
        public List<EventLite> FilterEvents(SearchEventByEntity eventSearch)
        {
            eventSearch.CountyCode = (string.IsNullOrEmpty(eventSearch.CountyCode) ? "GB" : eventSearch.CountyCode);
            List<EventLite> eventlist = new List<EventLite>();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:TPConnection"]))
            {
                var pointLatLong = eventSearch.Point.Split(",");
                SqlCommand command = new SqlCommand("GetEventsByFilter", connection);
                command.CommandTimeout = 240;
                command.CommandType = CommandType.StoredProcedure;
                eventSearch.RadiusFrom = 0;
                command.Parameters.Add(new SqlParameter("@latitude", pointLatLong.Length > 0 ? pointLatLong[0] : ""));
                command.Parameters.Add(new SqlParameter("@longitude", pointLatLong.Length > 1 ? pointLatLong[1] : ""));
                command.Parameters.Add(new SqlParameter("@radiusFrom", eventSearch.RadiusFrom));
                command.Parameters.Add(new SqlParameter("@radiusTo", eventSearch.RadiusTo));
                command.Parameters.Add(new SqlParameter("@eventStartDate", eventSearch.EventStartDate));
                command.Parameters.Add(new SqlParameter("@eventEndDate", eventSearch.EventEndDate));
                command.Parameters.Add(new SqlParameter("@sortBy", eventSearch.Sort));
                command.Parameters.Add(new SqlParameter("@countryCode", eventSearch.CountyCode));
                connection.Open();
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        EventLite eventLite = new EventLite
                        {
                            Id = reader.IsDBNull("Id") ? 0 : Convert.ToInt32(reader["Id"]),
                            EventStartUTC = reader.IsDBNull("EventStartUTC") ? DateTime.MinValue : Convert.ToDateTime(reader["EventStartUTC"]),
                            IsHotEvent = reader.IsDBNull("IsHotEvent") ? false : Convert.ToBoolean(reader["IsHotEvent"]),
                            Name = reader.IsDBNull("Name") ? "" : Convert.ToString(reader["Name"]),
                            StartTime = reader.IsDBNull("EventStartTime") ? "" : Convert.ToString(reader["EventStartTime"]),
                            Venue = reader.IsDBNull("VenueName") ? "" : Convert.ToString(reader["VenueName"]),
                            VenueId = reader.IsDBNull("VenueID") ? 0 : Convert.ToInt32(reader["VenueID"]),
                            City = reader.IsDBNull("City") ? "" : Convert.ToString(reader["City"]),
                            CountryCode = reader.IsDBNull("CountryCode") ? "" : Convert.ToString(reader["CountryCode"]),
                            TicketMinPrice = reader.IsDBNull("TicketMinPrice") ? 0 : Convert.ToInt32(reader["TicketMinPrice"]),
                            AvailableTicket = reader.IsDBNull("AvailableTickets") ? 0 : Convert.ToInt32(reader["AvailableTickets"]),
                            ImageURL = reader.IsDBNull("ImageURL") ? "" : Convert.ToString(reader["ImageURL"])
                        };
                        eventlist.Add(eventLite);
                    }
                }
            }
            return eventlist;
        }

        public List<GetEventsByVenueIdNewResponse> GetEventsByVenueIdNew(SearchEventByEntity request)
        {
            List<GetEventsByVenueIdNewResponse> eventlist = new List<GetEventsByVenueIdNewResponse>();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:TPConnection"]))
            {
                var pointLatLong = request.Point.Split(",");
                SqlCommand command = new SqlCommand("GetEventsByVenueId", connection);
                command.CommandTimeout = 240;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@venueId", request.EntityId));
                command.Parameters.Add(new SqlParameter("@radiusFrom", request.RadiusFrom));
                command.Parameters.Add(new SqlParameter("@radiusTo", request.RadiusTo));
                command.Parameters.Add(new SqlParameter("@startIndex", request.Start));
                command.Parameters.Add(new SqlParameter("@elimit", request.PageSize));
                command.Parameters.Add(new SqlParameter("@latitude", pointLatLong.Length > 0 ? pointLatLong[0] : ""));
                command.Parameters.Add(new SqlParameter("@longitude", pointLatLong.Length > 1 ? pointLatLong[1] : ""));
                command.Parameters.Add(new SqlParameter("@eventStartDate", request.EventStartDate));
                command.Parameters.Add(new SqlParameter("@eventEndDate", request.EventEndDate));
                command.Parameters.Add(new SqlParameter("@sortBy", request.Sort));
                connection.Open();
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        GetEventsByVenueIdNewResponse events = new GetEventsByVenueIdNewResponse();

                        events.Name = reader.IsDBNull("Name") ? "" : Convert.ToString(reader["Name"]);
                        events.Status = "";
                        events.Id = reader.IsDBNull("id") ? 0 : Convert.ToInt32(reader["id"]);
                        events.VenueId = reader.IsDBNull("venueId") ? 0 : Convert.ToInt32(reader["venueId"]);
                        events.VenueName = reader.IsDBNull("VenueName") ? "" : Convert.ToString(reader["VenueName"]);
                        events.ImageUrl = reader.IsDBNull("imageurl") ? "" : Convert.ToString(reader["imageurl"]);
                        events.StartTime = reader.IsDBNull("eventStartTime") ? "" : Convert.ToString(reader["eventStartTime"]);
                        events.Description = reader.IsDBNull("description") ? "" : Convert.ToString(reader["description"]);
                        events.EventDateLocal = reader.IsDBNull("eventstart") ? DateTime.MinValue : Convert.ToDateTime(reader["eventstart"]);
                        events.EventDateUTC = reader.IsDBNull("eventstartutc") ? DateTime.MinValue : Convert.ToDateTime(reader["eventstartutc"]);
                        events.MaxPrice = reader.IsDBNull("TicketMaxPrice") ? 0 : Convert.ToDecimal(reader["TicketMaxPrice"]);
                        events.MinPrice = reader.IsDBNull("TicketMinPrice") ? 0 : Convert.ToDecimal(reader["TicketMinPrice"]);
                        events.AvailableTicket = reader.IsDBNull("AvailableTickets") ? 0 : Convert.ToInt32(reader["AvailableTickets"]);
                        events.City = reader.IsDBNull("City") ? "" : Convert.ToString(reader["City"]);
                        events.Country = reader.IsDBNull("CountryCode") ? "" : Convert.ToString(reader["CountryCode"]);
                        eventlist.Add(events);
                    }
                }
            }
            return eventlist;
        }
        public List<GetEventsByVenueIdNewResponse> GetEventsBySearchNew(SearchEventByEntity request)
        {
            List<GetEventsByVenueIdNewResponse> eventlist = new List<GetEventsByVenueIdNewResponse>();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:TPConnection"]))
            {
                var pointLatLong = request.Point.Split(",");
                SqlCommand command = new SqlCommand("GetEventsBySearch", connection);
                command.CommandTimeout = 240;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@searchtext", request.FieldList));
                command.Parameters.Add(new SqlParameter("@radiusFrom", request.RadiusFrom));
                command.Parameters.Add(new SqlParameter("@radiusTo", request.RadiusTo));
                command.Parameters.Add(new SqlParameter("@startIndex", request.Start));
                command.Parameters.Add(new SqlParameter("@elimit", request.PageSize));
                command.Parameters.Add(new SqlParameter("@latitude", pointLatLong.Length > 0 ? pointLatLong[0] : ""));
                command.Parameters.Add(new SqlParameter("@longitude", pointLatLong.Length > 1 ? pointLatLong[1] : ""));
                command.Parameters.Add(new SqlParameter("@eventStartDate", request.EventStartDate));
                command.Parameters.Add(new SqlParameter("@eventEndDate", request.EventEndDate));
                command.Parameters.Add(new SqlParameter("@sortBy", request.Sort));

                connection.Open();
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        GetEventsByVenueIdNewResponse events = new GetEventsByVenueIdNewResponse();

                        events.Name = reader.IsDBNull("Name") ? "" : Convert.ToString(reader["Name"]);
                        events.Status = "";
                        events.Id = reader.IsDBNull("id") ? 0 : Convert.ToInt32(reader["id"]);
                        events.VenueId = reader.IsDBNull("venueId") ? 0 : Convert.ToInt32(reader["venueId"]);
                        events.VenueName = reader.IsDBNull("VenueName") ? "" : Convert.ToString(reader["VenueName"]);
                        events.ImageUrl = reader.IsDBNull("imageurl") ? "" : Convert.ToString(reader["imageurl"]);
                        events.StartTime = reader.IsDBNull("eventStartTime") ? "" : Convert.ToString(reader["eventStartTime"]);
                        events.Description = reader.IsDBNull("description") ? "" : Convert.ToString(reader["description"]);
                        events.EventDateLocal = reader.IsDBNull("eventstartutc") ? DateTime.MinValue : Convert.ToDateTime(reader["eventstartutc"]);
                        events.EventDateUTC = reader.IsDBNull("eventstartutc") ? DateTime.MinValue : Convert.ToDateTime(reader["eventstartutc"]);
                        events.MaxPrice = reader.IsDBNull("TicketMaxPrice") ? 0 : Convert.ToDecimal(reader["TicketMaxPrice"]);
                        events.MinPrice = reader.IsDBNull("TicketMinPrice") ? 0 : Convert.ToDecimal(reader["TicketMinPrice"]);
                        events.AvailableTicket = reader.IsDBNull("AvailableTickets") ? 0 : Convert.ToInt32(reader["AvailableTickets"]);
                        events.City = reader.IsDBNull("City") ? "" : Convert.ToString(reader["City"]);
                        events.Country = reader.IsDBNull("CountryCode") ? "" : Convert.ToString(reader["CountryCode"]);
                        events.Distance = reader.IsDBNull("distance") ? 0 : Math.Round(Convert.ToDecimal(reader["distance"]), 2);
                        eventlist.Add(events);
                    }
                }
            }
            return eventlist;
        }
        public List<EventLite> GetHotEvent(SearchEventByEntity eventSearch)
        {
            List<EventLite> eventlist = new List<EventLite>();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:TPConnection"]))
            {
                var pointLatLong = eventSearch.Point.Split(",");
                SqlCommand command = new SqlCommand("GetHotEvents", connection);
                command.CommandTimeout = 240;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@latitude", pointLatLong.Length > 0 ? pointLatLong[0] : ""));
                command.Parameters.Add(new SqlParameter("@longitude", pointLatLong.Length > 1 ? pointLatLong[1] : ""));
                command.Parameters.Add(new SqlParameter("@radiusFrom", eventSearch.RadiusFrom));
                command.Parameters.Add(new SqlParameter("@radiusTo", eventSearch.RadiusTo));
                command.Parameters.Add(new SqlParameter("@eventStartDate", eventSearch.EventStartDate));
                command.Parameters.Add(new SqlParameter("@eventEndDate", eventSearch.EventEndDate));
                command.Parameters.Add(new SqlParameter("@sortBy", eventSearch.Sort));
                command.Parameters.Add(new SqlParameter("@startIndex", eventSearch.Start));
                command.Parameters.Add(new SqlParameter("@elimit", eventSearch.PageSize));
                command.Parameters.Add(new SqlParameter("@countryCode", eventSearch.CountyCode));
                connection.Open();
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        EventLite eventLite = new EventLite
                        {
                            Id = reader.IsDBNull("Id") ? 0 : Convert.ToInt32(reader["Id"]),
                            EventStartUTC = reader.IsDBNull("EventStartUTC") ? DateTime.MinValue : Convert.ToDateTime(reader["EventStartUTC"]),
                            IsHotEvent = reader.IsDBNull("IsHotEvent") ? false : Convert.ToBoolean(reader["IsHotEvent"]),
                            Name = reader.IsDBNull("Name") ? "" : Convert.ToString(reader["Name"]),
                            StartTime = reader.IsDBNull("EventStartTime") ? "" : Convert.ToString(reader["EventStartTime"]),
                            Venue = reader.IsDBNull("VenueName") ? "" : Convert.ToString(reader["VenueName"]),
                            VenueId = reader.IsDBNull("VenueID") ? 0 : Convert.ToInt32(reader["VenueID"]),
                            City = reader.IsDBNull("City") ? "" : Convert.ToString(reader["City"]),
                            CountryCode = reader.IsDBNull("CountryCode") ? "" : Convert.ToString(reader["CountryCode"]),
                            TicketMinPrice = reader.IsDBNull("TicketMinPrice") ? 0 : Convert.ToDecimal(reader["TicketMinPrice"]),
                            AvailableTicket = reader.IsDBNull("AvailableTickets") ? 0 : Convert.ToInt32(reader["AvailableTickets"]),
                            ImageURL = reader.IsDBNull("ImageURL") ? "" : Convert.ToString(reader["ImageURL"])

                        };
                        eventlist.Add(eventLite);
                    }
                }
            }
            return eventlist;
        }
        public EventProductViewModel<ProductSale> GetEventInfo(long eventId)
        {
            return GetAll()
                .Include(x => x.Venue)
                .Where(x => x.Id == eventId)
                .Select(x => new EventProductViewModel<ProductSale>()
                {
                    Id = x.Id,
                    EventName = x.Name,
                    EventDate = x.EventStartUTC,
                    StartTime = x.EventStartUTC.ToString(@"hh\:mm"),
                    VenueName = x.Venue != null ? x.Venue.VenueName : string.Empty,
                    VenueCity = x.Venue != null ? x.Venue.City : string.Empty,
                    VenueCountry = x.Venue != null ? x.Venue.CountryCode : string.Empty,
                    AvailableTicket = x.TicketAvailable
                }).SingleOrDefault();
        }
    }
}