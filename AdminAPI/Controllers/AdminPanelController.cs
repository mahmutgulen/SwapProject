using Business.Abstract;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using DataAccess.Concrete.EntityFramework.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SwapAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminPanelController : ControllerBase
    {
        private IAdminService _adminService;

        public AdminPanelController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            var results = _adminService.GetUsers().Data;
            if (results != null)
            {
                return Ok(results);
            }
            return BadRequest();
        }



        [HttpGet("GetWallets")]
        public IActionResult GetWallets()
        {
            var results = _adminService.GetWallets().Data;
            if (results != null)
            {
                return Ok(results);
            }
            return BadRequest();
        }



        [HttpGet("GetOpenOrders")]
        public IActionResult GetOpenOrders()
        {
            var results = _adminService.GetOpenOrders().Data;
            if (results != null)
            {
                return Ok(results);
            }
            return BadRequest();
        }


        [HttpGet("GetCompanyVault")]
        public IActionResult GetCompanyVault()
        {
            var results = _adminService.GetCompanyVault().Data;
            if (results != null)
            {
                return Ok(results);
            }
            return BadRequest();
        }

        [HttpGet("GetUserAssets")]
        public IActionResult GetUserAssets()
        {
            var results = _adminService.GetUserAssets().Data;
            if (results != null)
            {
                return Ok(results);
            }
            return BadRequest();
        }


        [HttpGet("GetSuspendUsers")]
        public IActionResult GetSuspendUsers()
        {
            var results = _adminService.GetSuspendUsers();
            if (results != null)
            {
                return Ok(results);
            }
            return BadRequest();
        }


        [HttpPost("AddSuspend")]
        public IActionResult SuspendAccount(int UserId, string description)
        {
            var results = _adminService.SuspendAccount(UserId, description);
            if (results != null)
            {
                return Ok(results.Message);
            }
            return BadRequest(results.Message);
        }

        [HttpPost("RemoveSuspend")]
        public IActionResult RemoveSuspendAccount(int UserId)
        {
            var results = _adminService.RemoveSuspendAccount(UserId);
            if (results != null)
            {
                return Ok(results.Message);
            }
            return BadRequest(results.Message);
        }
    }
}
