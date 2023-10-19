using Core.Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SwapAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private SwapContext _context;

        public SearchController(SwapContext context)
        {
            _context = context;
        }

        [HttpGet("userSearch")]
        public async Task<ActionResult<IEnumerable<User>>> Search(string name)
        {
            IQueryable<User> query = _context.Users;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.UserName.Contains(name));
            }
            return Ok(query.ToList());
        }
    }
}
