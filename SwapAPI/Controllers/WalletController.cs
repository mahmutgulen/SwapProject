using Business.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SwapAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost("AddBalance")]
        public IActionResult AddBalance(int balance, string token)
        {
            var result = _walletService.AddBalance(balance, token);
            if (result != null)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("WithDrawBalance")]
        public IActionResult WithDrawBalance(int balance, string token)
        {
            var result = _walletService.WithDrawBalance(balance, token);
            if (result != null)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("GetBalance")]
        public IActionResult GetBalance(string token)
        {
            var result = _walletService.GetBalance(token);
            if (result != null)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
