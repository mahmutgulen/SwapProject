using Business.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SwapAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BTCUSDTController : ControllerBase
    {
        private IBtcUsdtService _btcUsdtService;

        public BTCUSDTController(IBtcUsdtService btcUsdtService)
        {
            _btcUsdtService = btcUsdtService;
        }

        [HttpPost("BuyLimiteOrder")]
        public IActionResult BuyLimiteOrder(string token, decimal buyLimit, decimal buyPrice)
        {
            var result = _btcUsdtService.BuyLimiteOrder(token, buyLimit, buyPrice);
            if (result != null)
            {
                return Ok(result.Message);
            }
            return BadRequest("ilk if bloğu başarılı bir şekidle çalıaştı0k");
        }


        [HttpPost("SellLimiteOrder2")]
        public IActionResult SellLimiteOrder(string token, decimal sellLimit, decimal sellAmount)
        {
            var result = _btcUsdtService.SellLimiteOrder(token, sellLimit, sellAmount);
            if (result != null)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

    }
}
