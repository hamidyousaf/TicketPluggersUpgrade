using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Response;

namespace TP.Upgrade.Application.Services
{
    public sealed class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IMapper _mapper;
        public CurrencyService(
            ICurrencyRepository currencyRepository,
            IMapper mapper)
        {
            _currencyRepository = currencyRepository;
            _mapper = mapper;
        }
        public async Task<List<CurrencyLite>> GetAll()
        {
            return await _currencyRepository.GetAllCurrencies().ToListAsync();
        }
        public async Task<List<CurrencyLite>> GetAllActiveCurrencies()
        {
            return await _currencyRepository.GetAllActiveCurrencies().ToListAsync();
        }
        public async Task<ResponseModel> GetAllCurrencies()
        {
            var data = await _currencyRepository.GetAllCurrencies().ToListAsync();

            return new ResponseModel()
            {
                IsSuccess = true,
                Data = data
            };
        }
    }
}
