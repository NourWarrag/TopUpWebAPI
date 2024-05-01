namespace TopUp.API.Models
{
    public class BeneficiaryLimitExceededException : Exception
    {
        public BeneficiaryLimitExceededException(string message) : base(message)
        {
        }
    }
}
