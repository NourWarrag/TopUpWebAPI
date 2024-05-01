namespace TopUp.API.Models
{
    public class TopUpTransaction
    {
        public Guid Id { get; set; }
        public Guid BeneficiaryId { get; set; }

        public Guid UserId { get; set; }
        public decimal Amount { get; set; }

        public bool isCharge { get; set; }
        public DateTime Date { get; set; }
    }
}
