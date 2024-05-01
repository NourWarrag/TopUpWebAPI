using System.ComponentModel.DataAnnotations;
using TopUp.API.Models;

namespace TopUp.API.DTO
{
    public class TopUpBeneficiaryCreateDTO
    {
        [Required]
        [StringLength(20)]
        public string NickName { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
