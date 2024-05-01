
namespace TopUp.API.Services.UserBalance
{
    public interface IUserBalanceService
    {
        Task DebitUserBalance(Guid userId, decimal amount);
        Task<decimal> GetUserBalance(Guid userId);
    }
}