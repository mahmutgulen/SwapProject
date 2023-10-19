using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.MarketPrice
{
    public class MarketPriceListDto : IDto
    {
        public decimal BTC { get; set; }
        public decimal ETH { get; set; }
        public decimal ADA { get; set; }
        public decimal TRX { get; set; }
        public decimal BNB { get; set; }
        public decimal DASH { get; set; }
        public decimal DOGE { get; set; }
        public decimal XRP { get; set; }
        public decimal SHIB { get; set; }
        public decimal UNI { get; set; }
    }
}
