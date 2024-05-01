namespace TopUp.API.Services.UserBalance
{
    public class UserBalanceContract
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
    }
}
