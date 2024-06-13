using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly IMapper _mapper;
        private readonly TP_DbContext _dbContext;
        public CustomerRepository(IMapper mapper,
            TP_DbContext dbContext) : base(dbContext)
        {
            _mapper = mapper;
            _dbContext= dbContext;
        }
        public async Task<bool> InsertCustomer(Customer customer, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return false;

            var result = await Add(customer);

            if (result is 0)
            {
                return false;
            }

            return true;
        }
        public IQueryable<CustomerLite> GetCustomerByUsername(string authorName)
        {
            return GetAll()
                .Where(x => x.Username == authorName)
                .ProjectTo<CustomerLite>(_mapper.ConfigurationProvider);
        }
        public IQueryable<long> GetCustomerIdByUserId(string userId)
        {
            return GetAll()
                .Where(x => x.UserId == userId)
                .Select(x => x.Id);
        }
        public IQueryable<GetCustomerProfileByIdDto> GetCustomerProfileById(long customerId)
        {
            return GetAll()
                .Include(x => x.BillingAddress)
                .Where(x => x.Id == customerId)
                .ProjectTo<GetCustomerProfileByIdDto>(_mapper.ConfigurationProvider);
        }
        public async Task<bool> IsBillingAddressExist(long customerId)
        {
            return await GetAll()
                .Select(x => new { x.Id, x.BillingAddressId})
                .AnyAsync(x => x.Id == customerId && x.BillingAddressId != null);
        }
        public async Task<bool> IsCustomerExist(long customerId)
        {
            return await GetAll().Select(x => x.Id).AnyAsync(x => x == customerId);
        }
        public IQueryable<Customer> GetCustomerWithUserByCustomerId(long customerId)
        {
            return GetAll()
                .Include(x => x.User)
                .Where(x => x.Id == customerId);
        }

        public IQueryable<GetOrdersByCustomerDto> GetOrdersByCustomerId(long customerId)
        {
            return _dbContext
                .Set<GetOrdersByCustomerDto>()
                .FromSqlRaw("GetOrdersByCustomer @customerId = {0}", customerId)
                .AsNoTracking();
        }

        public IQueryable<Customer> GetCustomerByEmail(string email)
        {
            return GetAll()
                .Where(x => x.Email.ToLower() == email.ToLower());
        }

        public IQueryable<Customer> GetVendorsInBulk(List<long> ids)
        {
            return GetAll()
                .Where(x => ids.Contains(x.Id) && x.IsVendor);
        }

        public IQueryable<Customer> GetCustomersInBulk(List<long> ids)
        {
            return GetAll()
                .IgnoreQueryFilters()
                .Where(x => ids.Contains(x.Id) && !x.IsVendor && !x.IsDeleted);
        }

        public IQueryable<Customer> GetCustomerWithCurrencyById(long customerId)
        {
            return GetReadOnlyList()
                .Include(x => x.Currency)
                .Where(x => x.Id == customerId);
        }
    }
}
