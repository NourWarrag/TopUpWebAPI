using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserBalance.API.DTO;
using UserBalance.API.Models;
using UserBalance.API.Services;

namespace UserBalance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBalanceController : ControllerBase
    {
        private readonly IUserBalanceService _userBalanceService;

        public UserBalanceController(IUserBalanceService userBalanceService)
        {
            _userBalanceService = userBalanceService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserBalance(Guid userId)
        {
            try
            {
                var userBalance = await _userBalanceService.GetUserBalanceAsync(userId);
                return Ok(userBalance);
            }
            catch (UserBalanceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("debit")]
        public async Task<IActionResult> DebitUserBalance(UserBalanceDebitCreditDTO debitDTO)
        {
            try
            {
                await _userBalanceService.DepitUserBalance(debitDTO.UserId, debitDTO.Amount);
                return Ok();
            }
            catch (UserBalanceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InsufficientBalanceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("credit")]
        public async Task<IActionResult> CreditUserBalance(UserBalanceDebitCreditDTO creditDTO)
        {
            try
            {
                await _userBalanceService.CreditUserBalance(creditDTO.UserId, creditDTO.Amount);
                return Ok();
            }
            catch (UserBalanceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        
       
    }
}
