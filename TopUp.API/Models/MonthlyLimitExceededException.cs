namespace TopUp.API.Models
{
    public class MonthlyLimitExceededException : Exception
    {
        public MonthlyLimitExceededException(string message) : base(message)
        {

        }
    }
}
