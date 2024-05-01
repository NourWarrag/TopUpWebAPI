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


            // Default users
            var verifiedTestUser = new User { Name = "VerifiedTestUser", PhoneNumber = "+971562552122" };
            var unverifiedTestUser = new User { Name = "UnverifiedTestUser", PhoneNumber = "+971562552123" };

            if (!_context.TopUpBeneficiaries.Any())
            {

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
