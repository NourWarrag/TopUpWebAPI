namespace TopUp.API.Models
{
    public class BeneficiaryAlreadyExistsException: Exception
    {
        public BeneficiaryAlreadyExistsException(string message): base(message)
        {
        }
    }
}
