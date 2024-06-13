using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Helpers.Pagination;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public sealed class EventService : IEventService
    {
        private readonly IEventStatusService _eventStatusService;
        private readonly ICategoryService _categoryService;
        private readonly IVenueService _venueService;
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly ICustomerFavouriteService _customerFavouriteServices;
        private readonly ISearchKeyService _searchKeyService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerFavouriteRepository _customerFavouriteRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IProductRepository _productRepository;

        public EventService(
            IEventStatusService eventStatusService,
            ICategoryService categoryService,
            IVenueService venueService,
            IEventRepository eventRepository,
            IMapper mapper,
            ICustomerFavouriteService customerFavouriteServices,
            ISearchKeyService searchKeyService,
            ICurrencyService currencyService,
            ICustomerRepository customerRepository,
            ICustomerFavouriteRepository customerFavouriteRepository,
            ICurrencyRepository currencyRepository,
            IProductRepository productRepository)
        {
            _eventStatusService = eventStatusService;
            _categoryService = categoryService;
            _venueService = venueService;
            _eventRepository = eventRepository;
            _mapper = mapper;
            _customerFavouriteServices = customerFavouriteServices;
            _searchKeyService = searchKeyService;
            _currencyService = currencyService;
            _customerRepository = customerRepository;
            _customerFavouriteRepository = customerFavouriteRepository;
            _currencyRepository = currencyRepository;
            _productRepository = productRepository;
        }
        public async Task<ResponseModel> InsertEvent(CreateEventRequest EventRequest, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            // check Event Status exist.
            var isEventStatusExist = await _eventStatusService.IsEventStatusExist(EventRequest.EventStatusId);
            if (!isEventStatusExist) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Event status not exist."
            };

            // check Segment exist.
            var isSegmentExist = await _categoryService.IsSegmentExist(EventRequest.SegmentId);
            if (!isSegmentExist) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Sagment not exist."
            };

            // check Genre exist.
            var isGenreExist = await _categoryService.IsGenreExist(EventRequest.SegmentId, EventRequest.GenreId);
            if (!isGenreExist) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Genre not exist."
            };

            // check Sub Genre exist.
            var isSubGenreExist = await _categoryService.IsSubGenreExist(EventRequest.GenreId, EventRequest.SubGenreId);
            if (!isSubGenreExist) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Sub Genre not exist."
            };

            // check Venue exist.
            var isVenueExist = await _venueService.IsVenueExist(EventRequest.VenueId);
            if (!isVenueExist) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Venue not exist."
            };

            // Add event
            var Event = _mapper.Map<Event>(EventRequest);
            var result = await _eventRepository.InsertEvent(Event);

            if (!result)
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "Something went wrong."
                };

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Event created successfully."
            };
        }
        public async Task<Event> GetById(int eventId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new Event();

            return await _eventRepository.Get(eventId);
        }
        public async Task<ResponseModel> GetEventStatuses()
        {
            var data = await _eventStatusService.GetEventStatuses();
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }

        public async Task<ResponseModel> GetGenresBySegmentId(short segmentId)
        {
            var data = await _categoryService.GetGenresBySegmentId(segmentId);
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }
        public async Task<ResponseModel> GetSubGenresByGenreId(short genreId)
        {
            var data = await _categoryService.GetSubGenresByGenreId(genreId);
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }
        public async Task<ResponseModel> GetCustomEvents()
        {
            var data = await _eventRepository.GetCustomEvents().ToListAsync();
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }
        public async Task<ResponseModel> DeleteCustomEvent(int id)
        {
            var record = await _eventRepository.Get(id);
            if (record is null)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "There is no event."
                };
            }
            await _eventRepository.SoftDelete(record);
            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Event has been deleted."
            };
        }

        public async Task<ResponseModel> GetEventsByText(string searchText)
        {
            List<GetEventByTextDto> data = new List<GetEventByTextDto>();

            if (!string.IsNullOrEmpty(searchText))
            {
                var query = _eventRepository.GetEventByText(searchText);
                data = await query.Take(10).ToListAsync();
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }
        public async Task<ResponseModel> SearchEvents(SearchEventRequest request)
        {
            var query = _eventRepository.SearchEvents(request.SearchText, request.StartDate, request.EndDate, request.EventId, request.CategoryId);
            var events = new PagedList<SearchEventDto>(query, request.PageIndex, request.PageSize, false);
            PaginationModel<SearchEventDto> model = new PaginationModel<SearchEventDto>();
            model.PagedContent = events;
            model.PagingFilteringContext.LoadPagedList(events);
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = model
            };
        }

        public async Task<ResponseModel> SearchSuggest(SearchSuggestRequest request)
        {
            SearchSuggestResultDto result = new SearchSuggestResultDto();
            List<CustomerFavouriteLite> custometfavorates = new List<CustomerFavouriteLite>();
            // Get customer favourites.
            if (request.CustomerId > 0)
            {
                custometfavorates = await _customerFavouriteServices.GetCustomerFavouritesAsync(request.CustomerId);
            }

            // Save search keyword 
            await _searchKeyService.AddOrUpdateSearchKey(request.SearchText);

            if (request.EntityList.ToLower().Contains("event"))
            {
                // Get customer favourite events
                var fevEvent = custometfavorates.Where(x => x.EventId > 0).Select(x => x.EventId).ToArray();
                // Get suggest events
                var query = _eventRepository.SearchSuggestEvents(request.SearchText);
                result.Events = await query.Take(request.PerfRows).ToListAsync();
                result.Events.OrderBy(x => x.EventStartUTC)
                    .OrderByDescending(x => x.TicketAvailable)
                    .ToList().ForEach(x => x.IsFavourits = fevEvent.Contains(x.Id) ? true : false);

                // Calculate currency
                if (request.CurrencyId > 0)
                {
                    var currency = await _currencyService.GetAll();
                    var customerCurrency = currency.Where(x => x.Id == request.CurrencyId).SingleOrDefault();
                    if (customerCurrency != null)
                    {
                        CurrencyLite eventCurrency;
                        foreach (var events in result.Events)
                        {
                            eventCurrency = currency.Where(x => x.CountryCode == events.CountryCode).SingleOrDefault();
                            var currencyBaseRate = eventCurrency.Rate / customerCurrency.Rate;
                            events.TicketMinPrice = Math.Round(events.TicketMinPrice * currencyBaseRate, 2);
                            events.Currency = customerCurrency.Symbol;
                        }
                    }
                }
            }

            if (request.EntityList.ToLower().Contains("venue"))
            {
                // Get customer favourite venues
                var fevVenue = custometfavorates.Where(x => x.VenueId > 0).Select(x => x.VenueId).ToArray();
                // Get suggest venues
                var query = await _venueService.SearchSuggestEvents(request.SearchText);
                result.Venues = query.Take(request.PerfRows).ToList();
                result.Venues.ToList()
                    .ForEach(x => x.IsFavourits = (fevVenue.Contains(x.Id) ? true : false));
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = result
            };
        }

        public async Task<ResponseModel> GetPopularSearchedEvent(GetPopularSearchedEventRequest request, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get data
            var data = await _eventRepository.GetPopularSearchedEvent(request).ToListAsync(ct);
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }

        public async Task<ResponseModel> GetEventById(GetEventByIdRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get event by id.
            var Event = await _eventRepository.GetEventById(request.EventId).FirstOrDefaultAsync(ct);
            if (Event is null) return new ResponseModel()
            {
                Message = $"Event not exists with Id {request.EventId}."
            };
            decimal customerCurrencyRate = 1;
            bool isFavourite = false;
            if (request.CustomerId > 0)
            {
                // get customer by id.
                var customer = await _customerRepository.GetCustomerWithCurrencyById(request.CustomerId).SingleOrDefaultAsync(ct);
                if (customer is null) return new ResponseModel()
                {
                    Message = $"Customer not exists with Id {request.CustomerId}."
                };

                // check customer currency exist.
                if (customer.Currency is null) return new ResponseModel()
                {
                    Message = $"Customer does not select currency."
                };

                // get event currency by currency code.
                var eventCurrency = await _currencyRepository.GetCurrencyByCountryCode(Event.CountryCode).SingleOrDefaultAsync(ct);
                if (eventCurrency is null) return new ResponseModel()
                {
                    Message = $"Event does not contain this currency."
                };

                // calculate min and max price by currency.
                customerCurrencyRate = eventCurrency.Rate / customer.Currency.Rate;
                Event.TicketMinPrice = Math.Round(Event.TicketMinPrice * customerCurrencyRate, 2);
                Event.TicketMaxPrice = Math.Round(Event.TicketMaxPrice * customerCurrencyRate, 2);

                // check customer favourite event.
                isFavourite = await _customerFavouriteRepository.IsFavouriteEventByCustomerId(request.CustomerId, request.EventId);
            }
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = new
                {
                    Event,
                    IsFavourite = isFavourite
                }
            };
        }

        public async Task<ResponseModel> GetEventsByVenueId(GetEventsByVenueIdRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get events by venue id.
            var events = _eventRepository.GetEventsByVenueId(request.VenueId);
            if (request.Sort == "EventStart")
            {
                events = events.OrderBy(x => x.EventStartUTC);
            }

            // Add pagination
            PaginationModel<GetEventsByVenueIdDto> model = new PaginationModel<GetEventsByVenueIdDto>();
            if (request.PageSize > 0)
            {
                var result = new PagedList<GetEventsByVenueIdDto>(events, request.PageIndex, request.PageSize, false);
                model.PagedContent = result;
                model.PagingFilteringContext.LoadPagedList(result);
            }
            else
            {
                model.PagedContent = await events.ToListAsync(ct);
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = model
            };
        }

        public async Task<ResponseModel> GetEventsBySearch(GetEventsBySearchRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get events by search.
            var events = _eventRepository.GetEventsBySearch(request.SearchText);
            if (request.Sort == "EventStart")
            {
                events = events.OrderBy(x => x.EventStartUTC);
            }

            // Add pagination
            PaginationModel<GetEventsBySearchDto> model = new PaginationModel<GetEventsBySearchDto>();
            if (request.PageSize > 0)
            {
                var result = new PagedList<GetEventsBySearchDto>(events, request.PageIndex, request.PageSize, false);
                model.PagedContent = result;
                model.PagingFilteringContext.LoadPagedList(result);
            }
            else
            {
                model.PagedContent = await events.ToListAsync(ct);
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = model
            };
        }

        public async Task<ResponseModel> GetEventListings(GetEventListingsRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get products by event id.
            var products = _productRepository.GetProductsByEventId(request.EventId);

            // Add pagination
            PaginationModel<ProductLite> model = new PaginationModel<ProductLite>();
            if (request.PageSize > 0)
            {
                var result = new PagedList<ProductLite>(products, request.PageIndex, request.PageSize, false);
                model.PagedContent = result;
                model.PagingFilteringContext.LoadPagedList(result);
            }
            else
            {
                model.PagedContent = await products.ToListAsync(ct);
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = model
            };
        }

        public async Task<ResponseModel> GetRecentHotEvent(GetRecentHotEventRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get recent top hot event by country code.
            var events = await _eventRepository.GetTopRecentHotEventByCountryCode(request.CountryCode).ToListAsync(ct);

            decimal customerCurrencyRate = 1;
            if (request.CustomerId > 0)
            {
                // get customer by id.
                var customer = await _customerRepository.GetCustomerWithCurrencyById(request.CustomerId).SingleOrDefaultAsync(ct);
                if (customer is null) return new ResponseModel()
                {
                    Message = $"Customer not exists with Id {request.CustomerId}."
                };

                // check customer currency exist.
                if (customer.Currency is null) return new ResponseModel()
                {
                    Message = $"Customer does not select currency."
                };

                // get event currency by currency code.
                var eventCurrency = await _currencyRepository.GetCurrencyByCountryCode(request.CountryCode).SingleOrDefaultAsync(ct);
                if (eventCurrency is null) return new ResponseModel()
                {
                    Message = $"Event does not contain this currency."
                };

                // calculate min and max price by currency.
                customerCurrencyRate = eventCurrency.Rate / customer.Currency.Rate;

                // check customer favourite event.
                var favourites = await _customerFavouriteRepository.GetFavouriteEventsByCustomerId(request.CustomerId).Select(x => x.EventId).ToListAsync(ct);

                events.ForEach(x =>
                {
                    x.TicketMinPrice = Math.Round(x.TicketMinPrice * customerCurrencyRate, 2);
                    x.TicketMaxPrice = Math.Round(x.TicketMinPrice * customerCurrencyRate, 2);
                    x.IsFavourits = favourites.Contains(x.Id) ? true: false;
                });
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = events
            };
        }

        public async Task<ResponseModel> GetFeaturedEvents(GetFeaturedEventRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            // get featured events by search and country code.
            var events = _eventRepository.GetFeaturedEventsBySearchAndCountryCode(request.SearchText, request.CountryCode);

            // Add pagination
            PaginationModel<EventLite> model = new PaginationModel<EventLite>();
            if (request.PageSize > 0)
            {
                var result = new PagedList<EventLite>(events, request.PageIndex, request.PageSize, false);
                model.PagedContent = result;
                model.PagingFilteringContext.LoadPagedList(result);
            }
            else
            {
                model.PagedContent = await events.ToListAsync(ct);
            }

            decimal customerCurrencyRate = 1;
            if (request.CustomerId > 0)
            {
                // get customer by id.
                var customer = await _customerRepository.GetCustomerWithCurrencyById(request.CustomerId).SingleOrDefaultAsync(ct);
                if (customer is null) return new ResponseModel()
                {
                    Message = $"Customer not exists with Id {request.CustomerId}."
                };

                // check customer currency exist.
                if (customer.Currency is null) return new ResponseModel()
                {
                    Message = $"Customer does not select currency."
                };

                // get event currency by currency code.
                var eventCurrency = await _currencyRepository.GetCurrencyByCountryCode(request.CountryCode).SingleOrDefaultAsync(ct);
                if (eventCurrency is null) return new ResponseModel()
                {
                    Message = $"Event does not contain this currency."
                };

                // calculate min and max price by currency.
                customerCurrencyRate = eventCurrency.Rate / customer.Currency.Rate;

                // check customer favourite event.
                var favourites = await _customerFavouriteRepository.GetFavouriteEventsByCustomerId(request.CustomerId).Select(x => x.EventId).ToListAsync(ct);

                model.PagedContent.ForEach(x =>
                {
                    x.TicketMinPrice = Math.Round(x.TicketMinPrice * customerCurrencyRate, 2);
                    x.TicketMaxPrice = Math.Round(x.TicketMinPrice * customerCurrencyRate, 2);
                    x.IsFavourits = favourites.Contains(x.Id) ? true : false;
                });
            }

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = model
            };
        }

        public async Task<EventOnLoadResponse> GetEventsOnLoad(GetEventsOnLoadRequest request, CancellationToken ct = default)
        {
            // get events.
            var result = _eventRepository.GetEventsOnLoad(request);
            if (result.Count() > 0)
            {
                decimal customerCurrencyRate = 1;
                if (request.CustomerId > 0)
                {
                    List<CustomerFavouriteLite> custometfavorates = new List<CustomerFavouriteLite>();
                    // Get customer favourites.
                    custometfavorates = await _customerFavouriteServices.GetCustomerFavouritesAsync(request.CustomerId);
                    var currency = await _currencyService.GetAll();
                    var eventCurrency = currency.Where(x => x.CountryCode == result.FirstOrDefault().Country).SingleOrDefault();
                    var customer = await _customerRepository.Get(request.CustomerId);
                    if (customer != null)
                    {
                        var customerCurrency = currency.Where(x => x.Id == customer.CurrencyId).SingleOrDefault();
                        customerCurrencyRate = eventCurrency.Rate / customerCurrency.Rate;
                    }
                    result.ForEach(x => { x.MinPrice = Math.Round(x.MinPrice * customerCurrencyRate, 2); x.MaxPrice = Math.Round(x.MaxPrice * customerCurrencyRate, 2); x.isFavourits = (custometfavorates.Where(x => x.EventId > 0).Select(a => a.EventId).Contains(x.Id) ? true : false); });
                }
            }
            var data = new EventOnLoadResponse();
            data.Events = result;
            data.PopularKeywords = await _searchKeyService.GetTop10SearchKeys();
            data.Poster = await _categoryService.GetPoster();
            return data;
        }
        public async Task<EventOnLoadNewResponse> GetEventsOnLoadNew(GetEventsOnLoadRequest request, CancellationToken ct = default)
        {
            EventOnLoadNewResponse eventListPages = new EventOnLoadNewResponse();
            request.ELimit = request.ELimit * 2; //take 2 times data
            var result = _eventRepository.GetEventsOnLoadNew(request);
            if (result.Count() > 0)
            {
                if (result.Count() > request.ELimit / 2)
                {
                    eventListPages.IsEventsRemaning = true;  // to decide view more button need to show or not
                }
                else
                {
                    eventListPages.IsEventsRemaning = false;
                }
                eventListPages.EventList = result.Take(request.ELimit / 2).ToList();
                decimal customerCurrencyRate = 1;
                if (request.CustomerId > 0)
                {
                    List<CustomerFavouriteLite> custometfavorates = new List<CustomerFavouriteLite>();
                    // Get customer favourites.
                    custometfavorates = await _customerFavouriteServices.GetCustomerFavouritesAsync(request.CustomerId);
                    var currency = await _currencyService.GetAll();
                    var eventCurrency = currency.Where(x => x.CountryCode == result.FirstOrDefault().Country).SingleOrDefault();
                    var customer = await _customerRepository.Get(request.CustomerId);
                    if (customer != null)
                    {
                        var customerCurrency = currency.Where(x => x.Id == customer.CurrencyId).SingleOrDefault();
                        customerCurrencyRate = eventCurrency.Rate / customerCurrency.Rate;
                    }
                    result.ForEach(x => { x.MinPrice = Math.Round(x.MinPrice * customerCurrencyRate, 2); x.MaxPrice = Math.Round(x.MaxPrice * customerCurrencyRate, 2); x.isFavourits = (custometfavorates.Where(x => x.EventId > 0).Select(a => a.EventId).Contains(x.Id) ? true : false); });
                }
            }
            return eventListPages;
        }

        public async Task<List<EventLite>> UpcommingEvents(UpcommingEventRequest request, CancellationToken ct = default)
        {
            var skip = (request.page - 1) * request.limit;
            List<EventLite> events = new List<EventLite>();
            if (request.Point != "")
            {
                SearchEventByEntity searchEvent = new SearchEventByEntity();
                searchEvent.EventEndDate = null;
                searchEvent.EventStartDate = null;
                searchEvent.Point = request.Point;
                searchEvent.RadiusFrom = request.RadiusFrom;
                searchEvent.RadiusTo = request.RadiusTo;
                searchEvent.Sort = "";
                events = _eventRepository.FilterEvents(searchEvent).Skip(skip).Take(request.limit).ToList();
            }
            else
            {
                events = await _eventRepository.UpcomminEvents().Skip(skip).Take(request.limit).ToListAsync(ct);
            }
            return events;
        }
        public async Task<List<EventLite>> EventsToSellorDashBoard(EventsToSellorDashBoardRequest request, CancellationToken ct)
        {
            var skip = (request.page) * request.limit;
            List<EventLite> events = new List<EventLite>();
            if (request.SearchBy == "location" && request.Point != "")
            {
                SearchEventByEntity searchEvent = new SearchEventByEntity();
                searchEvent.EventEndDate = null;
                searchEvent.EventStartDate = null;
                searchEvent.Point = request.Point;
                searchEvent.RadiusFrom = request.RadiusFrom;
                searchEvent.RadiusTo = request.RadiusTo;
                searchEvent.Sort = "";
                searchEvent.CountyCode = request.CountyCode;
                events = _eventRepository.FilterEvents(searchEvent).ToList();
            }
            else
            {
                events = await _eventRepository.UpcomminEvents().Where(x => x.SegmentId == request.Genre).Skip(skip).Take(request.limit).ToListAsync(ct);
            }
            if (request.Filter == 1)//recent added events
            {
                events = events.Where(x => x.EventStartUTC > DateTime.UtcNow).OrderBy(x => x.EventStartUTC).ToList();
            }
            else // high demand low supply
            {
                events = events.OrderByDescending(x => x.EventSearchCount).Skip(skip).Take(request.limit).ToList();
            }
            return events;
        }
        public async Task<PaginationModel<GetEventsByVenueIdNewResponse>> GetEventsByVenueIdNew(SearchEventByEntity request, CancellationToken ct)
        {
            var result = _eventRepository.GetEventsByVenueIdNew(request).AsQueryable();
            var eventLite = new PagedList<GetEventsByVenueIdNewResponse>(result, request.Start, request.PageSize);
            //----calculate rate using customer choosed currency...
            decimal customerCurrencyRate = 1;
            if (request.CustomerId > 0)
            {
                var currency = await _currencyService.GetAll();
                var eventCurrency = currency.Where(x => x.CountryCode == result.FirstOrDefault().Country).SingleOrDefault();
                var customer = await _customerRepository.Get(request.CustomerId);
                if (customer != null)
                {
                    var customerCurrency = currency.Where(x => x.Id == customer.CurrencyId).SingleOrDefault();
                    customerCurrencyRate = eventCurrency.Rate / customerCurrency.Rate;
                }
                result.ToList().ForEach(x => { x.MinPrice = Math.Round(x.MinPrice * customerCurrencyRate, 2); x.MaxPrice = Math.Round(x.MaxPrice * customerCurrencyRate, 2); });
            }
            PaginationModel<GetEventsByVenueIdNewResponse> model = new PaginationModel<GetEventsByVenueIdNewResponse>();
            model.PagedContent = eventLite;
            model.PagingFilteringContext.LoadPagedList(eventLite);
            return model;
        }
        public async Task<PaginationModel<GetEventsByVenueIdNewResponse>> GetEventsBySearchNew(SearchEventByEntity request, CancellationToken ct)
        {
            var result = _eventRepository.GetEventsBySearchNew(request).AsQueryable();
            var eventLite = new PagedList<GetEventsByVenueIdNewResponse>(result, request.Start, request.PageSize);
            PaginationModel<GetEventsByVenueIdNewResponse> model = new PaginationModel<GetEventsByVenueIdNewResponse>();
            model.PagedContent = eventLite;
            model.PagingFilteringContext.LoadPagedList(eventLite);
            return model;
        }
        public async Task<List<EventLite>> GetRecentHotEventWithLimit(SearchEventByEntity request, CancellationToken ct)
        {
            var skip = request.PageSize * request.Start;
            decimal customerCurrencyRate = 1;
            var result = _eventRepository.GetHotEvent(request).ToList();
            if (request.Sort == "EventStart")
            {
                result = result.OrderBy(x => x.EventStartUTC).ToList();
            }
            if (request.CustomerId > 0 && result.Count() > 0)
            {

                List<CustomerFavouriteLite> custometfavorates = new List<CustomerFavouriteLite>();
                // Get customer favourites.
                custometfavorates = await _customerFavouriteServices.GetCustomerFavouritesAsync(request.CustomerId);
                var currency = await _currencyService.GetAll();
                var eventCurrency = currency.Where(x => x.CountryCode == result.FirstOrDefault().CountryCode).SingleOrDefault();
                var customer = await _customerRepository.Get(request.CustomerId);
                if (customer != null)
                {
                    var customerCurrency = currency.Where(x => x.Id == customer.CurrencyId).SingleOrDefault();
                    customerCurrencyRate = eventCurrency.Rate / customerCurrency.Rate;
                }
                result.ForEach(x => { x.TicketMinPrice = Math.Round(x.TicketMinPrice * customerCurrencyRate, 2); x.TicketMaxPrice= Math.Round(x.TicketMaxPrice * customerCurrencyRate, 2); x.IsFavourits = (custometfavorates.Where(x => x.EventId > 0).Select(a => a.EventId).Contains(x.Id) ? true : false); });
            }
            return result;
        }
    }
}