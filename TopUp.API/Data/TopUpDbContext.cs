using Microsoft.EntityFrameworkCore;
using TopUp.API.Models;

namespace TopUp.API.Data
{
    public class TopUpDbContext : DbContext
    {
        public TopUpDbContext(DbContextOptions<TopUpDbContext> options) : base(options)
        {
        }

        public DbSet<TopUpTransaction> TopUpTransactions { get; set; }
        public DbSet<TopUpBeneficiary> TopUpBeneficiaries { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<TopUpOption> TopUpOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany<TopUpBeneficiary>()
                .WithOne()
                .HasForeignKey(b => b.UserId);

           modelBuilder.Entity<TopUpBeneficiary>()
                .HasMany<TopUpTransaction>()
                .WithOne()
                .HasForeignKey(t => t.BeneficiaryId);

            modelBuilder.Entity<TopUpBeneficiary>().Property(x => x.NickName).IsRequired();
            modelBuilder.Entity<TopUpBeneficiary>().Property(x => x.NickName).HasMaxLength(TopUpBeneficiary.NickNameMaxLength);
            modelBuilder.Entity<TopUpBeneficiary>().Property(x => x.UserId).IsRequired();
            modelBuilder.Entity<TopUpBeneficiary>().Property(x => x.PhoneNumber).IsRequired();

            modelBuilder.Entity<User>()
                .HasMany<TopUpTransaction>()
                .WithOne()
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<TopUpTransaction>().Property(x => x.Amount).HasPrecision(19, 4);

            modelBuilder.Entity<TopUpOption>().Property(x => x.Amount).HasPrecision(19, 4);

        }
    }
    
}
