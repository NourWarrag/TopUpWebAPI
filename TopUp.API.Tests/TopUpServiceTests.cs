using Microsoft.EntityFrameworkCore;
using Moq;
using TopUp.API.Data;
using TopUp.API.Exceptions;
using TopUp.API.Models;
using TopUp.API.Services;
using TopUp.API.Services.UserBalance;

public class TopUpServiceTests
{

    private readonly TopUpDbContext _context;
    private readonly TopUpService _service;
    private readonly Mock<IUserBalanceService> _mockBalanceService;

    public TopUpServiceTests()
    {
        var options = new DbContextOptionsBuilder<TopUpDbContext>()
            .UseInMemoryDatabase(databaseName: "TopUpTestDb")
            .Options;

        _context = new TopUpDbContext(options);
        _mockBalanceService = new Mock<IUserBalanceService>();
        _service = new TopUpService(_context, _mockBalanceService.Object);
    }

    [Fact]
    public async Task TopUp_WithInsufficientBalance_ShouldThrowException()
    {
        // Arrange
        var beneficiaryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var amount = 100m;
        var beneficiary = new TopUpBeneficiary { Id = beneficiaryId, NickName = "Test Beneficiary", PhoneNumber = "1234567890", UserId = userId };
        var user = new User { Id = userId, Name = "Test User", PhoneNumber = "1234567890" };

        _context.TopUpBeneficiaries.Add(beneficiary);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        _context.TopUpOptions.AddRange(new List<TopUpOption>
            {
               new() { Amount = 5  },
               new() { Amount = 10 },
               new() { Amount = 20 },
               new() { Amount = 30 },
               new() { Amount = 50 },
               new() { Amount = 75 },
               new() { Amount = 100}
            });

        await _context.SaveChangesAsync();
        _mockBalanceService.Setup(x => x.GetUserBalance(userId)).ReturnsAsync(amount - 1m);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InsufficientBalanceException>(() => _service.TopUp(amount, beneficiaryId, userId));
        Assert.Equal($"User {userId} has insufficient balance to top up {amount}.", exception.Message);
    }


    [Fact]
    public async Task TopUp_WithNonExistentBeneficiary_ShouldThrowException()
    {
        // Arrange
        var beneficiaryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var amount = 100m;

        var user = new User { Id = userId, Name = "Test User", PhoneNumber = "1234567890" };
        _context.TopUpOptions.AddRange(new List<TopUpOption>
            {
               new() { Amount = 5  },
               new() { Amount = 10 },
               new() { Amount = 20 },
               new() { Amount = 30 },
               new() { Amount = 50 },
               new() { Amount = 75 },
               new() { Amount = 100}
            });

        await _context.SaveChangesAsync();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _mockBalanceService.Setup(x => x.GetUserBalance(userId)).ReturnsAsync(amount + 1m);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BeneficiaryNotFoundException>(() => _service.TopUp(amount, beneficiaryId, userId));
        Assert.Equal($"Beneficiary not found.", exception.Message);
    }

    [Fact]
    public async Task TopUp_WithNonExistentUser_ShouldThrowException()
    {
        // Arrange
        var beneficiaryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var amount = 100m;

        var beneficiary = new TopUpBeneficiary { Id = beneficiaryId, NickName = "Test Beneficiary", PhoneNumber = "1234567890", UserId = userId};
        _context.TopUpOptions.AddRange(new List<TopUpOption>
            {
               new() { Amount = 5  },
               new() { Amount = 10 },
               new() { Amount = 20 },
               new() { Amount = 30 },
               new() { Amount = 50 },
               new() { Amount = 75 },
               new() { Amount = 100}
            });

        await _context.SaveChangesAsync();
        _context.TopUpBeneficiaries.Add(beneficiary);
        await _context.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UserNotFoundException>(() => _service.TopUp(amount, beneficiaryId, userId));
        Assert.Equal($"User not found.", exception.Message);
    }
    [Fact]
    public async Task TopUp_WithMonthlyLimitExceededForBeneficiary_forVerifiedAndUnVerifiedUsers_ShouldThrowException()
    {
        // Arrange
        var verifiedUserBeneficiaryId = Guid.NewGuid();

        var unVerifiedUserBeficiaryId = Guid.NewGuid();
        var verifiedUserId = Guid.NewGuid();
        var unVerifiedUserId = Guid.NewGuid();
        var amount = 100m;

        var beneficiary1 = new TopUpBeneficiary { Id = verifiedUserBeneficiaryId, NickName = "verifiedUserB", PhoneNumber = "1234567890", UserId = verifiedUserId };
        var beneficiary2 = new TopUpBeneficiary { Id = unVerifiedUserBeficiaryId, NickName = "unVerifiedUserB", PhoneNumber = "1234567890", UserId = unVerifiedUserId };

        var beneficiaries = new List<TopUpBeneficiary> { beneficiary1, beneficiary2 };
        var verifiedUser = new User { Id = verifiedUserId, Name = "Test User", PhoneNumber = "1234567890", IsVerified = true };
        var unVerifiedUser = new User { Id = unVerifiedUserId, Name = "Test User", PhoneNumber = "1234567890", IsVerified = false };

        _context.TopUpBeneficiaries.AddRange(beneficiaries);
        _context.Users.AddRange([verifiedUser, unVerifiedUser]);
        _context.TopUpOptions.AddRange(new List<TopUpOption>
            {
               new() { Amount = 5  },
               new() { Amount = 10 },
               new() { Amount = 20 },
               new() { Amount = 30 },
               new() { Amount = 50 },
               new() { Amount = 75 },
               new() { Amount = 100}
            });

        await _context.SaveChangesAsync();
        // Add transactions to exceed the monthly limit

        for (int i = 0; i < 10; i++)
        {
            _context.TopUpTransactions.Add(new TopUpTransaction { UserId = verifiedUserId, Amount = 100m, Date = DateTime.Now, BeneficiaryId = verifiedUserBeneficiaryId});
        }
        for (int i = 0; i < 5; i++)
        {
            _context.TopUpTransactions.Add(new TopUpTransaction { UserId = unVerifiedUserId, Amount = 100m, Date = DateTime.Now, BeneficiaryId = unVerifiedUserBeficiaryId});
        }



        await _context.SaveChangesAsync();

        _mockBalanceService.Setup(x => x.GetUserBalance(verifiedUserId)).ReturnsAsync(amount + 1m);
        _mockBalanceService.Setup(x => x.GetUserBalance(unVerifiedUserId)).ReturnsAsync(amount + 1m);

        // Act & Assert
        var verifiedUserException = await Assert.ThrowsAsync<MonthlyLimitExceededException>(() => _service.TopUp(amount, verifiedUserBeneficiaryId, verifiedUserId));
        var unVerifiedUserException = await Assert.ThrowsAsync<MonthlyLimitExceededException>(() => _service.TopUp(amount, unVerifiedUserBeficiaryId, unVerifiedUserId));
        Assert.Equal($"Monthly limit exceeded For verifiedUserB", verifiedUserException.Message);
        Assert.Equal($"Monthly limit exceeded For unVerifiedUserB", unVerifiedUserException.Message);
    }

    [Fact]
    public async Task TopUp_WithMonthlyLimitExceeded_ShouldThrowException()
    {
        // Arrange
        var beneficiary1Id = Guid.NewGuid();
        var beneficiary2Id = Guid.NewGuid();
        var beneficiary3Id = Guid.NewGuid();
        var beneficiary4Id = Guid.NewGuid();
        var beneficiary5Id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var amount = 100m;

        var beneficiary1 = new TopUpBeneficiary { Id = beneficiary1Id, NickName = "Test Beneficiary1", PhoneNumber = "1234567890", UserId = userId };
        var beneficiary2 = new TopUpBeneficiary { Id = beneficiary2Id, NickName = "Test Beneficiary2", PhoneNumber = "1234567890", UserId = userId };
        var beneficiary3 = new TopUpBeneficiary { Id = beneficiary3Id, NickName = "Test Beneficiary3", PhoneNumber = "1234567890", UserId = userId };
        var beneficiary4 = new TopUpBeneficiary { Id = beneficiary4Id, NickName = "Test Beneficiary4", PhoneNumber = "1234567890", UserId = userId };
        var beneficiary5 = new TopUpBeneficiary { Id = beneficiary5Id, NickName = "Test Beneficiary5", PhoneNumber = "1234567890", UserId = userId };
        var beneficiaries = new List<TopUpBeneficiary> { beneficiary1, beneficiary2, beneficiary3, beneficiary4, beneficiary5 };
        var user = new User { Id = userId, Name = "Test User", PhoneNumber = "1234567890", IsVerified = true };

        _context.TopUpBeneficiaries.AddRange(beneficiaries);
        _context.Users.Add(user);
        _context.TopUpOptions.AddRange(new List<TopUpOption>
            {
               new() { Amount = 5  },
               new() { Amount = 10 },
               new() { Amount = 20 },
               new() { Amount = 30 },
               new() { Amount = 50 },
               new() { Amount = 75 },
               new() { Amount = 100}
            });

        await _context.SaveChangesAsync();
        // Add transactions to exceed the monthly limit
        foreach (var b in beneficiaries)
        {
            for (int i = 0; i < 6; i++)
            {
                _context.TopUpTransactions.Add(new TopUpTransaction { UserId = userId, Amount = 100m, Date = DateTime.Now, BeneficiaryId = b.Id });
            }
        }


        await _context.SaveChangesAsync();

        _mockBalanceService.Setup(x => x.GetUserBalance(userId)).ReturnsAsync(amount + 1m);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<MonthlyLimitExceededException>(() => _service.TopUp(amount, beneficiary1Id, userId));
        Assert.Equal($"Monthly limit exceeded", exception.Message);
    }

    [Fact]
    public async Task TopUp_WithValidData_ShouldTopUpSuccessfully()
    {
        // Arrange
        var beneficiaryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var amount = 100m;

        var beneficiary = new TopUpBeneficiary { Id = beneficiaryId, NickName = "Test Beneficiary", PhoneNumber = "1234567890", UserId = userId };
        var user = new User { Id = userId, Name = "Test User", PhoneNumber = "1234567890" };

        _context.TopUpBeneficiaries.Add(beneficiary);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        _context.TopUpOptions.AddRange(new List<TopUpOption>
            {
               new() { Amount = 5  },
               new() { Amount = 10 },
               new() { Amount = 20 },
               new() { Amount = 30 },
               new() { Amount = 50 },
               new() { Amount = 75 },
               new() { Amount = 100}
            });

        await _context.SaveChangesAsync();

        _mockBalanceService.Setup(x => x.GetUserBalance(userId)).ReturnsAsync(amount + 1m);


        // Act
        await _service.TopUp(amount, beneficiaryId, userId);

        // Assert
        var updatedUser = await _context.Users.FindAsync(userId);
        var updatedBeneficiary = await _context.TopUpBeneficiaries.FindAsync(beneficiaryId);
        var transaction = await _context.TopUpTransactions.FirstOrDefaultAsync(t => t.UserId == userId && t.BeneficiaryId == beneficiaryId);

        Assert.NotNull(transaction);
    }


    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
