using Business.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SwapAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancelController : ControllerBase
    {
        private ICancelService _cancelService;

        public CancelController(ICancelService cancelService)
        {
            _cancelService = cancelService;
        }


        [HttpPost("CancelProcess")]
        public IActionResult Cancel(string token, int openOrderId)
        {
            var result = _cancelService.CancelProcess(token, openOrderId);
            if (result != null)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
