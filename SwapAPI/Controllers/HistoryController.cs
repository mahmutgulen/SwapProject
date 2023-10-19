using Business.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SwapAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpPost("GetMyTransactions")]
        public IActionResult History(string token)
        {
            var result = _historyService.GetMyTransactions(token);
            if (result!=null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
