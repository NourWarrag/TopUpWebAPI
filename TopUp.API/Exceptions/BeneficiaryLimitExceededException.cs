namespace TopUp.API.Exceptions
{
    public class BeneficiaryLimitExceededException : Exception
    {
        public BeneficiaryLimitExceededException(string message) : base(message)
        {
        }
    }
}
