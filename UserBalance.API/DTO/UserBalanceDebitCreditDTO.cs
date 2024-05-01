
using System.ComponentModel.DataAnnotations;

namespace UserBalance.API.DTO
{
    public class UserBalanceDebitCreditDTO
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
