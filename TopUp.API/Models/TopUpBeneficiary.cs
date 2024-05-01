namespace TopUp.API.Models
{
    public class TopUpBeneficiary
    {
        internal static int NickNameMaxLength = 20;

        public Guid Id { get; set; }
        public required string NickName { get; set; }

        public Guid UserId { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
