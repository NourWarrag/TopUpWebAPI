using System.Net;
using TopUp.API.Models;

namespace TopUp.API.Services.UserBalance
{
    public class UserBalanceService : IUserBalanceService
    {
        private readonly HttpClient _httpClient;
        public UserBalanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUserBalance(Guid userId)
        {
            var balance = await _httpClient.GetFromJsonAsync<UserBalanceContract>($"{userId}");

            return balance.Balance;
        }

        public async Task DebitUserBalance(Guid userId, decimal amount)
        {

            var response = await _httpClient.PostAsJsonAsync("debit", new
            {
                UserId = userId,
                Amount = amount
            });

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new UserBalanceNotFoundException(message);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new InsufficientBalanceException(message);
                }
            }
        }
    }
}
