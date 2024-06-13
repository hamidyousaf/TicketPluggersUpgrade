using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Common.Contracts.IDataManagers
{
    public interface ICurrencyRepository : IRepository<Currency>
    {
        IQueryable<CurrencyLite> GetAllCurrencies();
        IQueryable<CurrencyLite> GetAllActiveCurrencies();
        IQueryable<CurrencyLite> GetCurrencyByCountryCode(string countryCode);
    }
}
