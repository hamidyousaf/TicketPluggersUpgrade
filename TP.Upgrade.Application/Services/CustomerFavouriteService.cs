using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Enums;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public sealed class CustomerFavouriteService : ICustomerFavouriteService
    {
        private readonly ICustomerFavouriteRepository _customerFavouriteRepository;
        private readonly IMapper _mapper;
        private readonly ISharedService _sharedService;
        private readonly IVenueService _venueService;
        public CustomerFavouriteService(
            ICustomerFavouriteRepository customerFavouriteRepository,
            IMapper mapper,
            ISharedService sharedService,
            IVenueService venueService)
        {
            _customerFavouriteRepository = customerFavouriteRepository;
            _mapper = mapper;
            _sharedService = sharedService;
            _venueService = venueService;
        }
        public async Task<ResponseModel> InsertCustomerFavourite(CreateCustomerFavouriteRequest favouriteRequest, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            var alreadyExist = new CustomerFavouriteLite();
            if (favouriteRequest.EventId > 0)
            {
                // check event exist.
                var isEventExist = await _sharedService.IsEventExist((int)favouriteRequest.EventId);
                if (!isEventExist)
                {
                    return new ResponseModel()
                    {
                        IsSuccess = false,
                        Message = "Event does not exist."
                    };
                }

                // check favourite already exist or not.
                alreadyExist = await _customerFavouriteRepository
                    .GetCustomerFavouritesByEventId(favouriteRequest.CustomerId, (int)favouriteRequest.EventId)
                    .SingleOrDefaultAsync(ct);

                favouriteRequest.FavouriteType = Convert.ToByte(FavouriteType.Event);
            }
            else if (favouriteRequest.VenueId > 0)
            {
                // check venue exist.
                var isVenueExist = await _venueService.IsVenueExist((int)favouriteRequest.VenueId);
                if (!isVenueExist)
                {
                    return new ResponseModel()
                    {
                        IsSuccess = false,
                        Message = "Venue does not exist."
                    };
                }

                // check favourite already exist or not.
                alreadyExist = await _customerFavouriteRepository
                    .GetCustomerFavouritesByVenueId(favouriteRequest.CustomerId, (int)favouriteRequest.VenueId)
                    .SingleOrDefaultAsync(ct);

                favouriteRequest.FavouriteType = Convert.ToByte(FavouriteType.Venue);
            }
            else
            {
                // check favourite already exist or not.
                alreadyExist = await _customerFavouriteRepository
                    .GetCustomerFavouritesByPerformerId(favouriteRequest.CustomerId, (int)favouriteRequest.PerformerId)
                    .SingleOrDefaultAsync(ct);

                favouriteRequest.FavouriteType = Convert.ToByte(FavouriteType.Performer);
            }

            if (alreadyExist is not null)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "You already added this in favourite."
                };
            }

            // Map the object
            var favourite = _mapper.Map<CustomerFavourite>(favouriteRequest);
            favourite.IsFavourite = true;
            favourite.CreatedDate = DateTime.UtcNow;
            var result = await _customerFavouriteRepository.InsertCustomerFavourite(favourite, ct);

            if (!result)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "Customer's favourite not added."
                };
            }
            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Customer's favourite successfully added."
            };
        }
        public async Task<List<CustomerFavouriteLite>> GetCustomerFavouritesAsync(long customerId, CancellationToken ct)
        {
            return await _customerFavouriteRepository.GetCustomerFavourites(customerId).ToListAsync(ct);
        }
        public async Task<ResponseModel> ChangeNotifyStatus(FavouriteNotifyRequest notifyRequest, CancellationToken ct = default)
        {
            // get by id.
            var favourite = await _customerFavouriteRepository
                .GetCustomerFavouriteById(notifyRequest.CustomerId, notifyRequest.Id)
                .SingleOrDefaultAsync(ct);

            // Change status
            if (favourite is not null)
            {
                favourite.NotifyStatus = !favourite.NotifyStatus;
                await _customerFavouriteRepository.Change(favourite);

                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "Notify Status successfully changed."
                };
            }

            return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Customer's favourite not found."
            };
        }
        public async Task<ResponseModel> DeleteCustomerFavourite(DeleteCustomerFavouriteRequest favouriteRequest, CancellationToken ct = default)
        {
            // get by id.
            var favourite = await _customerFavouriteRepository
                .GetCustomerFavouriteById(favouriteRequest.CustomerId, favouriteRequest.Id)
                .SingleOrDefaultAsync(ct);

            // Delete customer favourite
            if (favourite is not null)
            {
                await _customerFavouriteRepository.Delete(favourite.Id);

                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "Customer's favourite successfully deleted."
                };
            }

            return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Customer's favourite not found."
            };
        }

        public async Task<ResponseModel> GetFavourites(GetFavouriteRequest favouriteRequest, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                IsSuccess = false,
                Message = "Your request is cancelled."
            };

            GetFavouriteResultDto favouriteResult = new GetFavouriteResultDto();
            if (favouriteRequest.Type.ToLower().Contains("event"))
            {
                // Get customer's favourite events.
                favouriteResult.FavouriteEvents = await _customerFavouriteRepository
                    .GetCustomerFavouriteEventsBySearch(favouriteRequest.CustomerId, favouriteRequest.SearchString)
                    .ToListAsync(ct);
            }
            else if (favouriteRequest.Type.ToLower().Contains("performer"))
            {
                //    //var custometfavorates = _customerFavouritesRepository.Table.Where(x => x.PerformerId > 0 && x.CustomerId == request.CustomerId && !x.IsDeleted && x.IsFavourite).Select(x => x.PerformerId).ToArray();
                //    // var custometfavorates = _customerFavouritesRepository.Table.Where(x => x.PerformerId > 0 && x.CustomerId == request.CustomerId && !x.IsDeleted && x.IsFavourite).Select(x => new { Id = x.Id, Id2 = x.PerformerId, notifyStatus = x.NotifyStatus }).ToList();
                //    // var Ids = custometfavorates.Select(x => x.Id2).Distinct().ToArray();

                //    favouriteResult.Performers = (from att in _attractionRepository.Table
                //                                join fav in _customerFavouritesRepository.Table on att.Id equals fav.PerformerId
                //                                where /*Ids.Contains(venue.Id) && */ att.Name.Contains(request.SearchString)
                //                                && fav.CustomerId == request.CustomerId && !fav.IsDeleted && fav.IsFavourite
                //                                select new PerformerInfo()
                //                                {
                //                                    Id = fav.Id,
                //                                    Id2 = att.Id,
                //                                    Attraction_ID = att.Attraction_ID,
                //                                    Name = att.Name,
                //                                    ImageURL = att.ImageURL,
                //                                    SubType = att.SubType,
                //                                    NotifyStatus = fav.NotifyStatus
                //                                }).ToList();
                //    //searchSuggest.performers.ToList().ForEach(x => x.Id = custometfavorates.Where(y => y.Id2 == x.Id).Select(x => x.Id).FirstOrDefault());

            }
            else
            {
                // Get customer's favourite venues.
                favouriteResult.FavouriteVenues = await _customerFavouriteRepository
                    .GetCustomerFavouriteVenuesBySearch(favouriteRequest.CustomerId, favouriteRequest.SearchString)
                    .ToListAsync(ct);
            }
            return new ResponseModel()
            {
                IsSuccess = true,
                Data = favouriteResult
            };
        }
    }
}