namespace TopUp.API.Exceptions
{
    public class UserBalanceNotFoundException : Exception
    {
        public UserBalanceNotFoundException(string message) : base(message)
        {
        }
    }
}
