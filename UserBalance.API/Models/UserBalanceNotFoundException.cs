namespace UserBalance.API.Models
{
    public class UserBalanceNotFoundException : Exception
    {
        public UserBalanceNotFoundException(string message) : base(message) { }
    }
}
