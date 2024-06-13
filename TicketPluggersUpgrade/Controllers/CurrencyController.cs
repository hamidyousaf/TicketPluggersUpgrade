using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.Services;

namespace TP.Upgrade.Api.Controllers
{
    public class CurrencyController : ApiBaseController
    {
        private readonly ICurrencyService _currencyService;
        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        [AllowAnonymous, HttpGet("GetAllCurrencies")]
        public async Task<IActionResult> GetAllCurrencies()
        {
            return Ok(await _currencyService.GetAllCurrencies());
        }
        [AllowAnonymous, HttpGet("GetAllActiveCurrencies")]
        public async Task<IActionResult> GetAllActiveCurrencies()
        {
            return Ok(await _currencyService.GetAllActiveCurrencies());
        }
    }
}
