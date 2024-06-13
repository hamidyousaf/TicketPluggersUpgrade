using AutoMapper;
using AutoMapper.QueryableExtensions;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public sealed class CurrencyRepository : Repository<Currency>, ICurrencyRepository
    {
        private readonly IMapper _mapper;
        public CurrencyRepository(
            TP_DbContext _dbContext,
            IMapper mapper) : base(_dbContext)
        {
            _mapper = mapper;
        }

        public IQueryable<CurrencyLite> GetAllCurrencies()
        {
            return GetReadOnlyList()
                .ProjectTo<CurrencyLite>(_mapper.ConfigurationProvider);
        }
        public IQueryable<CurrencyLite> GetAllActiveCurrencies()
        {
            return GetReadOnlyList()
                .Where(x => x.IsPublished)
                .ProjectTo<CurrencyLite>(_mapper.ConfigurationProvider);
        }

        public IQueryable<CurrencyLite> GetCurrencyByCountryCode(string countryCode)
        {
            return GetReadOnlyList()
                .Where(x => x.IsPublished && x.CountryCode.ToLower() == countryCode.ToLower())
                .ProjectTo<CurrencyLite>(_mapper.ConfigurationProvider);

        }
    }
}
