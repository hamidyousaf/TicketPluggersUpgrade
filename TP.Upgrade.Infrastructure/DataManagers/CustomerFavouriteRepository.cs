using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class CustomerFavouriteRepository : Repository<CustomerFavourite>, ICustomerFavouriteRepository
    {
        private readonly IMapper _mapper;
        public CustomerFavouriteRepository(
            TP_DbContext _dbContext,
            IMapper mapper) : base(_dbContext)
        {
            _mapper = mapper;
        }
        public async Task<bool> InsertCustomerFavourite(CustomerFavourite customerFavourite, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return false;

            var result = await Add(customerFavourite);

            if (result is 0)
            {
                return false;
            }

            return true;
        }
        public IQueryable<CustomerFavouriteLite> GetCustomerFavourites(long customerId)
        {
            return GetAll()
                .Where(x => x.CustomerId == customerId && x.IsFavourite)
                .ProjectTo<CustomerFavouriteLite>(_mapper.ConfigurationProvider);
        }
        public IQueryable<CustomerFavouriteLite> GetCustomerFavouritesByEventId(long customerId, int eventId)
        {
            return GetAll()
                .Where(x => x.CustomerId == customerId && x.EventId == eventId)
                .ProjectTo<CustomerFavouriteLite>(_mapper.ConfigurationProvider);
        }
        public IQueryable<CustomerFavouriteLite> GetCustomerFavouritesByVenueId(long customerId, int venueId)
        {
            return GetAll()
                .Where(x => x.CustomerId == customerId && x.VenueId == venueId)
                .ProjectTo<CustomerFavouriteLite>(_mapper.ConfigurationProvider);
        }
        public IQueryable<CustomerFavouriteLite> GetCustomerFavouritesByPerformerId(long customerId, int performerId)
        {
            return GetAll()
                .Where(x => x.CustomerId == customerId && x.PerformerId == performerId)
                .ProjectTo<CustomerFavouriteLite>(_mapper.ConfigurationProvider);
        }
        public IQueryable<CustomerFavourite> GetCustomerFavouriteById(long customerId, int favouriteId)
        {
            return GetAll()
                .Where(x => x.CustomerId == customerId && x.Id == favouriteId);
        }
        public IQueryable<GetFavouriteEventDto> GetCustomerFavouriteEventsBySearch(long customerId, string searchText)
        {
            return GetAll()
                .Include(x => x.Event)
                    .ThenInclude(x => x.Venue)
                .Where(x => x.Event.Name.ToLower().Contains(searchText.ToLower()) && x.CustomerId == customerId && x.EventId != null)
                .ProjectTo<GetFavouriteEventDto>(_mapper.ConfigurationProvider);
        }
        public IQueryable<GetFavouriteVenueDto> GetCustomerFavouriteVenuesBySearch(long customerId, string searchText)
        {
            return GetAll()
                .Include(x => x.Venue)
                .Where(x => x.Venue.VenueName.ToLower().Contains(searchText.ToLower()) && x.CustomerId == customerId && x.VenueId != null)
                .ProjectTo<GetFavouriteVenueDto>(_mapper.ConfigurationProvider);
        }
        public async Task<bool> IsFavouriteEventByCustomerId(long customerId, long eventId)
        {
            return GetReadOnlyList()
                .Any(x => x.CustomerId == customerId && x.EventId == eventId && x.IsFavourite);
        }

        public IQueryable<CustomerFavouriteLite> GetFavouriteEventsByCustomerId(long customerId)
        {
            return GetReadOnlyList()
                .Where(x => x.CustomerId == customerId && x.IsFavourite && x.EventId != null)
                .ProjectTo<CustomerFavouriteLite>(_mapper.ConfigurationProvider);
        }
    }
}