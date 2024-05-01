using System.ComponentModel.DataAnnotations;

namespace TopUp.API.DTO
{
    public class TopUpBeneficiaryUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [StringLength(20)]
        public string NickName { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
