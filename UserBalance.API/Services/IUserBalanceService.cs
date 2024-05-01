using UserBalance.API.Models;

namespace UserBalance.API.Services
{
    public interface IUserBalanceService
    {
        Task CreditUserBalance(Guid userId, decimal amount);
        Task DepitUserBalance(Guid userId, decimal amount);
        Task<UserBalanceModel> GetUserBalanceAsync(Guid userId);
    }
}