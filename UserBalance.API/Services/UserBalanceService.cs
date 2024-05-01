using Microsoft.EntityFrameworkCore;
using UserBalance.API.Data;
using UserBalance.API.Models;

namespace UserBalance.API.Services
{
    public class UserBalanceService : IUserBalanceService
    {
        private readonly UserBalanceContext _context;
        public UserBalanceService(UserBalanceContext context)
        {
            _context = context;
        }

        public async Task<UserBalanceModel> GetUserBalanceAsync(Guid userId)
        {

            return await _context.UserBalances.FirstOrDefaultAsync(x => x.UserId == userId) ?? throw new UserBalanceNotFoundException("User Balance Not Found");

        }

        public async Task DepitUserBalance(Guid userId, decimal amount)
        {
            var userBalance = await _context.UserBalances.FirstOrDefaultAsync(x => x.UserId == userId) ?? throw new UserBalanceNotFoundException("User Balance Not Found");

            if (userBalance.Balance < amount)
            {
                throw new InsufficientBalanceException("Insufficient Balance");
            }

            userBalance.Balance -= amount;

            await _context.SaveChangesAsync();
        }

        public async Task CreditUserBalance(Guid userId, decimal amount)
        {
            var userBalance = await _context.UserBalances.FirstOrDefaultAsync(x => x.UserId == userId) ?? throw new UserBalanceNotFoundException("User Balance Not Found");

            userBalance.Balance += amount;

            await _context.SaveChangesAsync();
        }

    }
}
