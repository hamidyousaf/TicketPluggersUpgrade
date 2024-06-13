using AutoMapper;
using AutoMapper.QueryableExtensions;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class AddressRepository : Repository<Address>, IAddressRepository
    {
        private readonly IMapper _mapper;
        public AddressRepository(IMapper mapper,
            TP_DbContext _dbContext) : base(_dbContext)
        {
            _mapper = mapper;
        }
        public async Task<bool> InsertAddress(Address address, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return false;

            var result = await Add(address);

            if (result is 0)
            {
                return false;
            }

            return true;
        }
        public IQueryable<AddressLite> GetAddressesByCustomerId(long customerId)
        {
            return GetAll()
                .Where(x => x.CustomerId == customerId)
                .ProjectTo<AddressLite>(_mapper.ConfigurationProvider);
        }

        public IQueryable<AddressLite> GetAddressByIdAndCustomerId(int addressId, long customerId)
        {
            return GetAll()
                .Where(x => x.Id == addressId && x.CustomerId == customerId)
                .ProjectTo<AddressLite>(_mapper.ConfigurationProvider);
        }
    }
}
