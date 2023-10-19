using Business.Abstract;
using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace SwapAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketPriceController : ControllerBase
    {
        private readonly IMarketPriceService _marketPriceService;

        public MarketPriceController(IMarketPriceService marketPriceService)
        {
            _marketPriceService = marketPriceService;
        }

        [HttpGet("GetParite")]
        public IActionResult GetParite(string parite)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol={parite}").Result;
            var responseBody = response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Parite>(responseBody.Result);

            return Ok(data);
        }

        [HttpGet("GetCoins")]
        public IActionResult GetMarketPrice()
        {
            var result = _marketPriceService.GetCoins();
            if (result!=null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

    }
}
