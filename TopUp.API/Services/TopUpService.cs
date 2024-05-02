using Microsoft.EntityFrameworkCore;
using TopUp.API.Data;
using TopUp.API.Exceptions;
using TopUp.API.Models;
using TopUp.API.Services.UserBalance;

namespace TopUp.API.Services
{
    public class TopUpService : ITopUpService
    {
        private const decimal VerifiedUserBeneficiaryMonthlyLimit = 1_000m;
        private const decimal unVerifiedUserBeneficiaryMonthlyLimit = 500m;
        private const decimal TotalMonthlyLimit = 3_000m;

        private readonly TopUpDbContext _context;
        private readonly IUserBalanceService _balanceService;

        public TopUpService(TopUpDbContext context, IUserBalanceService balanceService)
        {
            _context = context;
            _balanceService = balanceService;
        }

        public async Task TopUp(decimal amount, Guid beneficiaryId, Guid userId)
        {
             if (amount <= 0) throw new InvalidAmountException("Invalid amount");
            
            var topUpOption = await _context.TopUpOptions.FirstOrDefaultAsync(x => x.Amount == amount) ?? throw new TopUpOptionNotFoundException("Top up option not found.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException("User not found.");

            var beneficiary = await _context.TopUpBeneficiaries.FirstOrDefaultAsync(x => x.Id == beneficiaryId && x.UserId == userId ) ?? throw new BeneficiaryNotFoundException("Beneficiary not found.");

            await CheckIfUserExceededMonthlyLimitForBeneficiary(amount, beneficiary, user);

            await CheckIfTheUserExceededTotalLimit(amount, userId);

            var userBalance = await _balanceService.GetUserBalance(userId);
            if (userBalance < amount + 1m)
            {
                throw new InsufficientBalanceException($"User {userId} has insufficient balance to top up {amount}.");
            }

            await _balanceService.DebitUserBalance(userId, amount + 1m);

            var transaction = new TopUpTransaction
            {
                Id = Guid.NewGuid(),
                BeneficiaryId = beneficiaryId,
                UserId = userId,
                Amount = amount,
                Date = DateTime.UtcNow
            };

            await _context.TopUpTransactions.AddAsync(transaction);

            var chargeTransaction = new TopUpTransaction
            {
                Id = Guid.NewGuid(),
                BeneficiaryId = beneficiaryId,
                UserId = userId,
                Amount = 1,
                isCharge = true,
                Date = DateTime.UtcNow
            };
            await _context.TopUpTransactions.AddAsync(chargeTransaction);

            await _context.SaveChangesAsync();


        }

        private async Task CheckIfTheUserExceededTotalLimit(decimal amount, Guid userId)
        {
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var totalTopUpThisMonth = await _context.TopUpTransactions
                .Where(t => t.UserId == userId && t.Date >= startOfMonth)
                .SumAsync(t => t.Amount);

            if (totalTopUpThisMonth + amount > TotalMonthlyLimit)
            {
                throw new MonthlyLimitExceededException("Monthly limit exceeded");
            }
        }

        private async Task CheckIfUserExceededMonthlyLimitForBeneficiary(decimal amount, TopUpBeneficiary beneficiary, User user)
        {

            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var totalTopUpThisMonth = await _context.TopUpTransactions
                .Where(t => t.UserId == user.Id && t.BeneficiaryId == beneficiary.Id && t.Date >= startOfMonth)
                .SumAsync(t => t.Amount);

            var monthlyLimit = user.IsVerified ? VerifiedUserBeneficiaryMonthlyLimit : unVerifiedUserBeneficiaryMonthlyLimit;

            if (totalTopUpThisMonth + amount > monthlyLimit)
            {
                throw new MonthlyLimitExceededException($"Monthly limit exceeded For {beneficiary.NickName}");
            }
        }

        public async Task<List<TopUpOption>> GetTopUpOptions()
        {
            return await _context.TopUpOptions.ToListAsync();
        }

        public async Task<TopUpBeneficiary> GetTopUpBeneficiary(Guid id)
        {
            var beneficiary = await _context.TopUpBeneficiaries.FirstOrDefaultAsync(x => x.Id == id);
            return beneficiary ?? throw new BeneficiaryNotFoundException("Beneficiary not found");
        }

        public async Task<List<TopUpBeneficiary>> GetUserTopUpBeneficiaries(Guid userId)
        {
            return await _context.TopUpBeneficiaries.Where(b => b.UserId == userId).ToListAsync();
        }

        public async Task<TopUpBeneficiary> AddTopUpBeneficiary(TopUpBeneficiary beneficiary)
        {
            var existingUserBeneficiary = await _context.TopUpBeneficiaries.FirstOrDefaultAsync(x => x.UserId == beneficiary.UserId && x.PhoneNumber == beneficiary.PhoneNumber);
            if (existingUserBeneficiary != null)
            {
                throw new BeneficiaryAlreadyExistsException("A beneficiary with the same phone number already exists.");
            };
            var existingUserBenificiariesCount = await _context.TopUpBeneficiaries.CountAsync(x => x.UserId == beneficiary.UserId);
            if (existingUserBenificiariesCount >= 5)
            {
                throw new BeneficiaryLimitExceededException("already added the maximum number of beneficiaries.");
            }

            await _context.TopUpBeneficiaries.AddAsync(beneficiary);
            await _context.SaveChangesAsync();
            return beneficiary;
        }

        public async Task<TopUpBeneficiary> UpdateTopUpBeneficiary(TopUpBeneficiary beneficiary)
        {
            _context.TopUpBeneficiaries.Update(beneficiary);
            await _context.SaveChangesAsync();
            return beneficiary;
        }

        public async Task<TopUpBeneficiary> DeleteTopUpBeneficiary(Guid id)
        {
            var beneficiary = await _context.TopUpBeneficiaries.FirstOrDefaultAsync(x => x.Id == id) ?? throw new BeneficiaryNotFoundException("Beneficiary not found");
            _context.TopUpBeneficiaries.Remove(beneficiary);
            await _context.SaveChangesAsync();
            return beneficiary;
        }

    }
}
