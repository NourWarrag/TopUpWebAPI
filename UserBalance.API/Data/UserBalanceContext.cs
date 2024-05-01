using Microsoft.EntityFrameworkCore;
using UserBalance.API.Models;

namespace UserBalance.API.Data
{
    public class UserBalanceContext : DbContext
    {
        public UserBalanceContext(DbContextOptions<UserBalanceContext> options) : base(options)
        {
        }

        public DbSet<UserBalanceModel> UserBalances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserBalanceModel>().Property(x => x.Balance).HasPrecision(19, 4);
        }
    }
}
