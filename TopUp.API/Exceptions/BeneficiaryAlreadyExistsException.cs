namespace TopUp.API.Exceptions
{
    public class BeneficiaryAlreadyExistsException : Exception
    {
        public BeneficiaryAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
