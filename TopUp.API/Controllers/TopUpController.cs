using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TopUp.API.DTO;
using TopUp.API.Models;
using TopUp.API.Services;

namespace TopUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopUpController : ControllerBase
    {
        private readonly ITopUpService _topUpService;

        public TopUpController(ITopUpService topUpService)
        {
            _topUpService = topUpService;
        }

        [HttpGet("User/{userId}/Beneficiaries/")]
        public async Task<IActionResult> GetBeneficiaries(Guid userId)
        {
            var beneficiaries = await _topUpService.GetUserTopUpBeneficiaries(userId);
            return Ok(beneficiaries);
        }

        [HttpGet("topup_options")]
        public async Task<IActionResult> GetTopUpOptions()
        {
            var options = await _topUpService.GetTopUpOptions();
            return Ok(options);
        }
        [HttpPost("Beneficiaries")]
        public async Task<IActionResult> AddBeneficiary(TopUpBeneficiaryCreateDTO beneficiaryDto)
        {
            var beneficiary = new TopUpBeneficiary
            {
                Id = Guid.NewGuid(),
                NickName = beneficiaryDto.NickName,
                PhoneNumber = beneficiaryDto.PhoneNumber
            };
            var addedBeneficiary = await _topUpService.AddTopUpBeneficiary(beneficiary);
            return Ok(addedBeneficiary);
        }

        [HttpGet("Beneficiaries/{id}")]
        public async Task<IActionResult> GetBeneficiary(Guid id)
        {
            var beneficiary = await _topUpService.GetTopUpBeneficiary(id);
            return Ok(beneficiary);
        }

        [HttpPut("Beneficiaries/{id}")]
        public async Task<IActionResult> UpdateBeneficiary(Guid id, TopUpBeneficiaryUpdateDTO beneficiaryDto)
        {
            if (id != beneficiaryDto.Id)
            {
                return BadRequest();
            }
            var beneficiary = new TopUpBeneficiary
            {
                Id = id,
                NickName = beneficiaryDto.NickName,
                PhoneNumber = beneficiaryDto.PhoneNumber
            };
            var updatedBeneficiary = await _topUpService.UpdateTopUpBeneficiary(beneficiary);
            return Ok(updatedBeneficiary);
        }

        [HttpDelete("Beneficiaries/{id}")]
        public async Task<IActionResult> DeleteBeneficiary(Guid id)
        {
            await _topUpService.DeleteTopUpBeneficiary(id);
            return Ok();
        }

        [HttpPost("TopUp")]
        public async Task<IActionResult> TopUp(decimal amount, Guid beneficiaryId, Guid userId)
        {

            await _topUpService.TopUp(amount, beneficiaryId, userId);
            return Ok();
        }

    }
}
