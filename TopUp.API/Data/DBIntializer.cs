using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TopUp.API.Models;

namespace TopUp.API.Data
{
    public static class InitialiserExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<DBIntializer>();

            await initialiser.InitialiseAsync();

            await initialiser.SeedAsync();
        }
    }
    public class DBIntializer
    {
        private readonly ILogger<DBIntializer> _logger;
        private readonly TopUpDbContext _context;

        public DBIntializer(ILogger<DBIntializer> logger, TopUpDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {


            if(!_context.Users.Any())
            {
                var verifiedTestUser = new User { Id = new Guid("6CF37BBC-68A0-4E16-9122-3B58D65A8F64"), Name = "VerifiedTestUser", PhoneNumber = "+971562552122", IsVerified = true };
                var unverifiedTestUser = new User { Id = new Guid("49AFBE99-EB5E-4E82-AA7D-5116D71268CA"), Name = "UnverifiedTestUser", PhoneNumber = "+971562552123", IsVerified = false };

                _context.Users.AddRange(new List<User> { verifiedTestUser, unverifiedTestUser });

                await _context.SaveChangesAsync();
            }
            if (!_context.TopUpBeneficiaries.Any())
            {
                _context.TopUpBeneficiaries.AddRange(new List<TopUpBeneficiary>
                {
                    new TopUpBeneficiary { UserId = new Guid("6CF37BBC-68A0-4E16-9122-3B58D65A8F64"), NickName = "TestBeneficiary", PhoneNumber = "+971562552124" },
                    new TopUpBeneficiary { UserId = new Guid("49AFBE99-EB5E-4E82-AA7D-5116D71268CA"), NickName = "TestBeneficiary", PhoneNumber = "+971562552125" },

                });
                await _context.SaveChangesAsync();
            }

            if (!_context.TopUpOptions.Any())
            {

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
            }
        }
    }
}
