namespace TopUp.API.Exceptions
{
    public class MonthlyLimitExceededException : Exception
    {
        public MonthlyLimitExceededException(string message) : base(message)
        {

        }
    }
}
