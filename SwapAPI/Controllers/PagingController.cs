using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SwapAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagingController : ControllerBase
    {
        [HttpGet("Users")]
        public IActionResult Paging(int activePage = 1)
        {
            int pageSize = 1;
            using SwapContext context = new SwapContext();
            var products = context.Users
                .OrderBy(x => x.UserId)
                .Skip((activePage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int userCount = context.Users.Count();
            double pageCount = (double)((decimal)userCount / Convert.ToDecimal(pageSize));

            PagingModel model = new PagingModel();
            model.TotalCount = userCount;
            model.PageSize = pageSize;
            model.ActivePage = activePage;
            model.Users = products;
            model.TotalPageCount = (int)Math.Ceiling(pageCount);

            return Ok(model);
        }


        [HttpGet("OpenOrders")]
        public IActionResult Coins(int activePage = 1)
        {
            int pageSize = 100;
            using SwapContext context = new SwapContext();
            var openOrders = context.OpenOrders
                .OrderBy(x => x.Id)
                .Skip((activePage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int orderCount = context.OpenOrders.Count();
            double pageCount = (double)((decimal)orderCount / Convert.ToDecimal(pageSize));

            PagingModel model = new PagingModel();
            model.TotalCount = orderCount;
            model.PageSize = pageSize;
            model.ActivePage = activePage;
            model.OpenOrders = openOrders;
            model.TotalPageCount = (int)Math.Ceiling(pageCount);

            return Ok(model);
        }

        [HttpGet("GetTopTransactions")]
        public IActionResult TopTransactions(int activePage = 1)
        {
            int pageSize = 100;
            using SwapContext context = new SwapContext();
            var openOrders = context.OpenOrders
                .OrderByDescending(x => x.Price)//çoktan aza
                .Skip((activePage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int orderCount = context.OpenOrders.Count();
            double pageCount = (double)((decimal)orderCount / Convert.ToDecimal(pageSize));

            PagingModel model = new PagingModel();
            model.TotalCount = orderCount;
            model.PageSize = pageSize;
            model.ActivePage = activePage;
            model.OpenOrders = openOrders;
            model.TotalPageCount = (int)Math.Ceiling(pageCount);

            return Ok(model);
        }
    }
}
