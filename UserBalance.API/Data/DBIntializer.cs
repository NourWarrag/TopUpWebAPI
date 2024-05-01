using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using UserBalance.API.Models;

namespace UserBalance.API.Data
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
        private readonly UserBalanceContext _context;

        public DBIntializer(ILogger<DBIntializer> logger, UserBalanceContext context)
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

            if (!_context.UserBalances.Any())
            {

                _context.UserBalances.AddRange(new List<UserBalanceModel>
            {
               new() { UserId = new Guid("49AFBE99-EB5E-4E82-AA7D-5116D71268CA"), Balance = 2_500  },
               new() {UserId = new Guid("6CF37BBC-68A0-4E16-9122-3B58D65A8F64"), Balance = 3_500 },
              
            });

                await _context.SaveChangesAsync();
            }
        }
    }
}
