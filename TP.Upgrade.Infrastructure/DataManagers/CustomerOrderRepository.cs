using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Enums;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class CustomerOrderRepository : Repository<CustomerOrder>, ICustomerOrderRepository
    {
        private readonly IMapper _mapper;
        public CustomerOrderRepository(
            IMapper mapper,
            TP_DbContext dbContext) : base(dbContext)
        {
            _mapper = mapper;
        }
        public IQueryable<CustomerOrderLite> GetOrdersByVendorId(long vendorId)
        {
            return GetReadOnlyList()
                .Where(x => x.VendorId == vendorId)
                .ProjectTo<CustomerOrderLite>(_mapper.ConfigurationProvider);
        }
        public async Task<bool> InsertCustomerOrder(CustomerOrder order, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return false;

            var result = await Add(order);

            if (result is 0)
            {
                return false;
            }

            return true;
        }

        public IQueryable<GetOrderByIdDto> GetOrderById(long orderId, long customerId, bool readOnly = true)
        {
            var query = GetAll();
            if (readOnly)
            {
                query = GetReadOnlyList();
            }
            return query
                .AsSplitQuery()
                .Include(x => x.Customer)
                .Include(x => x.Vendor)
                .Include(x => x.Product)
                    .ThenInclude(x => x.Event)
                        .ThenInclude(x => x.Venue)
                .Where(x => x.Id == orderId && x.CustomerId == customerId)
                .ProjectTo<GetOrderByIdDto>(_mapper.ConfigurationProvider);
        }
        public IQueryable<GetOrderByIdDto> GetOrderById(long orderId, bool readOnly = true)
        {
            var query = GetAll();
            if (readOnly)
            {
                query = GetReadOnlyList();
            }
            return query
                .AsSplitQuery()
                .Include(x => x.Customer)
                .Include(x => x.Vendor)
                .Include(x => x.Product)
                    .ThenInclude(x => x.Event)
                        .ThenInclude(x => x.Venue)
                .Where(x => x.Id == orderId)
                .ProjectTo<GetOrderByIdDto>(_mapper.ConfigurationProvider);
        }

        public IQueryable<GetOrderByIdDto> GetSalesOrdersByCustomerId(long customerId, bool readOnly = true)
        {
            var query = GetAll();
            if (readOnly)
            {
                query = GetReadOnlyList();
            }
            return query
                .AsSplitQuery()
                .Include(x => x.Customer)
                .Include(x => x.Vendor)
                .Include(x => x.Product)
                    .ThenInclude(x => x.Event)
                        .ThenInclude(x => x.Venue)
                .Where(x => x.VendorId == customerId)
                .OrderByDescending(s => s.CreatedDate)
                .ProjectTo<GetOrderByIdDto>(_mapper.ConfigurationProvider);
        }

        public IQueryable<GetOrderByIdDto> GetSalesOrderForAdmin(bool readOnly = true)
        {
            var query = GetAll();
            if (readOnly)
            {
                query = GetReadOnlyList();
            }
            return query
                .AsSplitQuery()
                .Include(x => x.Customer)
                .Include(x => x.Vendor)
                .Include(x => x.Product)
                    .ThenInclude(x => x.Event)
                        .ThenInclude(x => x.Venue)
                .OrderByDescending(s => s.CreatedDate)
                .ProjectTo<GetOrderByIdDto>(_mapper.ConfigurationProvider);
        }

        public IQueryable<GetOrderByIdDto> GetOrdersByCustomerId(long customerid)
        {
            var query = GetReadOnlyList();
            return query
                .AsSplitQuery()
                .Include(x => x.Customer)
                .Include(x => x.Vendor)
                .Include(x => x.Product)
                    .ThenInclude(x => x.Event)
                        .ThenInclude(x => x.Venue)
                .Where(x => x.CustomerId == customerid)
                .OrderByDescending(s => s.CreatedDate)
                .ProjectTo<GetOrderByIdDto>(_mapper.ConfigurationProvider);
        }

        public IQueryable<CustomerOrder> GetOrdersByOrderIds(List<long> orderIds)
        {
            return GetAll()
                .Where(x => orderIds.Contains(x.Id));
        }

        public IQueryable<CustomerOrder> GetCompletedOrdersByVendorId(long vendorId)
        {
            return GetAll()
                .Where(x => x.OrderStatusId == (byte)OrderStatus.PaymentCompleted && x.VendorId == vendorId);
        }

        public IQueryable<GetOrderByIdDto> GetOrders()
        {
            return GetReadOnlyList()
                .Include(x => x.Product)
                    .ThenInclude(x => x.Event)
                .OrderByDescending(x => x.CreatedDate)
                .ProjectTo<GetOrderByIdDto>(_mapper.ConfigurationProvider);
        }

        public IQueryable<MessageSessionDto> GetOrdersByCustomerIdAndSearchText(long userId, string searchText)
        {
            return GetReadOnlyList()
                .Include(x => x.Product)
                    .ThenInclude(x => x.Event)
                        .ThenInclude(x => x.Venue)
                .Where(x => x.CustomerId == userId && x.Id.ToString().Contains(searchText))
                        .ProjectTo<MessageSessionDto>(_mapper.ConfigurationProvider);
        }
        public IQueryable<MessageSessionDto> GetOrdersByVendorIdAndSearchText(long userId, string searchText)
        {
            return GetReadOnlyList()
                .Include(x => x.Product)
                    .ThenInclude(x => x.Event)
                        .ThenInclude(x => x.Venue)
                .Where(x => x.VendorId == userId && x.Id.ToString().Contains(searchText))
                        .ProjectTo<MessageSessionDto>(_mapper.ConfigurationProvider);
        }

        public IQueryable<GetOrderByIdDto> GetOrderByPaymentId(string paymentId)
        {
            return GetReadOnlyList()
                .AsSplitQuery()
                .Include(x => x.Customer)
                .Include(x => x.Vendor)
                .Include(x => x.Product)
                    .ThenInclude(x => x.Event)
                        .ThenInclude(x => x.Venue)
                .Where(x => x.PaymentId == paymentId)
                .OrderByDescending(s => s.CreatedDate)
                .ProjectTo<GetOrderByIdDto>(_mapper.ConfigurationProvider);
        }

        public IQueryable<GetOrdersByTicketTypeIdDto> GetOrdersByTicketTypeId(byte ticketTypeId)
        {
            return GetReadOnlyList()
                .Include(x => x.Product)
                    .ThenInclude(x => x.Event)
                .Include(x => x.Vendor)
                .Where(x => x.TimeExtensionRequestStatus == (byte)RequestStatus.Requested && x.TicketTypeId == ticketTypeId)
                .ProjectTo<GetOrdersByTicketTypeIdDto>(_mapper.ConfigurationProvider);
        }
        public IQueryable<ProductSale> getSaledTicketsForEvent(long eventId)
        {
            return GetAll()
                .Include(x => x.Product)
                    .ThenInclude(x => x.Event)
                        .ThenInclude(x => x.Venue)
                .Include(x => x.Customer)
                .Where(x => x.Product.EventId == eventId)
                .Select(x => new ProductSale
                {
                    Id = x.Product.Id,
                    Currency = x.Product.CurrencyCode,
                    FaceValue = x.Product.FaceValue,
                    Name = x.Product.Name,
                    eventId = x.Product.EventId,
                    ProceedCost = x.Product.ProceedCost,
                    Price = x.Product.Price,
                    Published = x.Product.IsPublished,
                    StockQuantity = x.Product.StockQuantity,
                    SoldQuantity = x.Product.SoldQuantity,
                    VendorId = x.Product.VendorId,
                    ticketRow = x.Product.TicketRow,
                    seatsFrom = x.Product.SeatsFrom,
                    seatsTo = x.Product.SeatsTo,
                    ticketTypeId = x.Product.TicketTypeId,
                    sectionId = x.Product.SectionId,
                    OrderId = x.Id,
                    OrderDate = x.CreatedDate
                });
        }
    }
}
