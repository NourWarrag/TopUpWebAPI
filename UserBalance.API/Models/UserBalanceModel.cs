namespace UserBalance.API.Models
{
    public class UserBalanceModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
    }
}
