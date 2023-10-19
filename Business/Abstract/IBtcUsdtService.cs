using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IBtcUsdtService
    {
        IResult BuyLimiteOrder(string token, decimal buyLimit, decimal buyPrice);
        IResult SellLimiteOrder(string token, decimal sellLimit, decimal sellAmount);
    }
}
