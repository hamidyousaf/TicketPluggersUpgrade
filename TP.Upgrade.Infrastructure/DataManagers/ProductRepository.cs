using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly IMapper _mapper;
        public ProductRepository(
            IMapper mapper,
            TP_DbContext dbContext) : base(dbContext)
        {
            _mapper = mapper;
        }

        public async Task<bool> InsertProduct(Product product, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return false;

            var result = await Add(product);

            if (result is 0)
            {
                return false;
            }

            return true;
        }
        public IQueryable<ProductLite> GetProductsByVendorId(long vendorId)
        {
            return GetReadOnlyList()
                .Where(x => x.VendorId == vendorId)
                .ProjectTo<ProductLite>(_mapper.ConfigurationProvider);
        }

        public IQueryable<ProductLite> GetProductsByEventId(int eventId)
        {
            return GetReadOnlyList()
                .Where(x => x.EventId == eventId)
                .ProjectTo<ProductLite>(_mapper.ConfigurationProvider);
        }

        public IQueryable<MessageSessionDto> GetProductsByVendorIdAndSearchText(long userId, string searchText)
        {
            return GetReadOnlyList()
                    .Include(x => x.Event)
                        .ThenInclude(x => x.Venue)
                .Where(x => x.VendorId == userId && x.Id.ToString().Contains(searchText))
                .ProjectTo<MessageSessionDto>(_mapper.ConfigurationProvider);
        }

        public IQueryable<ListingWrapper> GetAllEventListingsForAdmin(GetAllListingRequest request, decimal defaultCommissionRate)
        {
            var query = GetAll()
                .Include(x => x.Vendor)
                .Include(x => x.Event)
                    .ThenInclude(x => x.Venue)
                .Select(x => new ListingWrapper()
                {
                    Id = x.Id,
                    Currency = x.CurrencySymbol,
                    Price = x.Price,
                    Name = x.Name,
                    StockQuantity = x.StockQuantity,
                    SoldQuantity = x.SoldQuantity,
                    RemainingQuantity = x.StockQuantity - x.SoldQuantity,
                    ticketRow = x.TicketRow,
                    SectionId = x.SectionId,
                    eventId = x.EventId,
                    seatsTo = x.SeatsTo,
                    seatsFrom = x.SeatsFrom,
                    ticketTypeId = x.TicketTypeId,
                    Published = x.IsPublished,
                    Commission = defaultCommissionRate,
                    EventStart = x.Event.EventStartUTC,
                    EventName = x.Event.Name,
                    StartTime = x.Event.EventStartTime.ToString(@"hh\:mm"),
                    Venue = x.Event.Venue != null ? x.Event.Venue.VenueName : string.Empty,
                    VenueCity = x.Event.Venue != null ? x.Event.Venue.City : string.Empty,
                    VenueCountry = x.Event.Venue != null ? x.Event.Venue.CountryCode : string.Empty,
                    CreatedOn = x.CreatedDate,
                    UpdatedOn = x.UpdatedDate,
                    Deleted = x.IsDeleted,
                    VenueId = x.Event.Venue != null ? x.Event.Venue.Id : 0,
                    VendorFirstName = x.Vendor.FirstName,
                    VendorLastName = x.Vendor.LastName,
                    VendorId = x.Vendor.Id
                });

            if (!string.IsNullOrEmpty(request.deliveryTypeList))
            {
                List<int> productypeids = request.deliveryTypeList.Split(";").Select(int.Parse).ToList();
                query = query.Where(x => productypeids.Contains(x.ticketTypeId));
            }
            if (!string.IsNullOrWhiteSpace(request.searchText))
                query = query.Where(c => c.EventName.ToLower().Contains(request.searchText.ToLower()) || c.Venue.ToLower().Contains(request.searchText.ToLower()));


            if (request.fromDate != DateTime.MinValue)
                query = query.Where(c => c.EventStart >= request.fromDate && c.EventStart <= request.toDate);
            if (!string.IsNullOrWhiteSpace(request.countryCode))
            {
                query = query.Where(c => c.VenueCountry == request.countryCode);
            }
            if (!string.IsNullOrEmpty(request.sort))
            {
                switch (request.sort)
                {
                    case "event":
                        query = query = query.OrderBy(x => x.EventStart).AsQueryable();
                        break;
                    default:
                        query = query = query.OrderByDescending(x => x.CreatedOn).AsQueryable();
                        break;
                }

            }
            return query;
        }
        public IQueryable<ProductSale> getUploadedTicketsForEvent(long eventId)
        {
            return GetAll()
                .Include(x => x.Event)
                    .ThenInclude(x => x.Venue)
                .Where(x => x.EventId == eventId)
                .Select(product => new ProductSale
                {

                    Id = product.Id,
                    Currency = product.CurrencyCode,
                    FaceValue = product.FaceValue,
                    Name = product.Name,
                    eventId = product.EventId, // basically eventId
                    ProceedCost = product.ProceedCost,
                    Price = product.Price,
                    Published = product.IsPublished,
                    StockQuantity = product.StockQuantity,
                    SoldQuantity = product.SoldQuantity,
                    VendorId = product.VendorId,
                    ticketRow = product.TicketRow,
                    seatsFrom = product.SeatsFrom,
                    seatsTo = product.SeatsTo,
                    ticketTypeId = product.TicketTypeId,
                    sectionId = product.SectionId
                });
        }
    }
}
