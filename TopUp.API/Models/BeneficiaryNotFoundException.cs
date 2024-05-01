namespace TopUp.API.Models
{
    public class BeneficiaryNotFoundException : Exception
    {
        public BeneficiaryNotFoundException(string message) : base(message)
        {
        }
    }
}
