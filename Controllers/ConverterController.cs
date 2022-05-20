using CurrencyConverter.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConverterController : ControllerBase
    {
        private readonly ILogger<ConverterController> _logger;
        private readonly ICurrencyService _currencyService;

        public ConverterController(ILogger<ConverterController> logger, ICurrencyService currencyService)
        {
            _logger = logger;
            _currencyService = currencyService;
        }

        [HttpGet]
        [Route("Convert")]
        public async Task<decimal> Convert(string fromCurrency, string toCurrency, decimal amount)
        {
            try
            {
                return await _currencyService.ConvertCurrency(fromCurrency, toCurrency, amount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        [Route("UpdateConfiguration")]
        public async Task<bool> UpdateConfiguration()
        {
            try
            {
                await _currencyService.UpdateConfiguration();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}