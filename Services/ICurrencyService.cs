
namespace CurrencyConverter.Services
{
    public interface ICurrencyService
    {
        Task<decimal> ConvertCurrency(string fromCurrency, string toCurrency, decimal amount);
        Task UpdateConfiguration();
    }
}