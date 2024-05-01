using System.ComponentModel.DataAnnotations;

namespace TopUp.API.DTO
{
    public class TopUpRequestDTO
    {
        [Required]
        [Range(1, 1000)]
        public decimal Amount { get; set; }
        [Required]
        public Guid BeneficiaryId { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}
