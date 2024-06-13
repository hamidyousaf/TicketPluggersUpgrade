using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Response;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface ICurrencyService
    {
        Task<List<CurrencyLite>> GetAll();
        Task<List<CurrencyLite>> GetAllActiveCurrencies();
        Task<ResponseModel> GetAllCurrencies();
    }
}
