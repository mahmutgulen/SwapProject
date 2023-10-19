using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Pending : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Precision(18, 8)]
        public decimal BTC { get; set; }
        [Precision(18, 8)]
        public decimal USDT { get; set; }
        [Precision(18, 8)]
        public decimal TRX { get; set; }
        [Precision(18, 8)]
        public decimal ETH { get; set; }
        [Precision(18, 8)]
        public decimal Commission { get; set; }
        public int OpenOrdersId { get; set; }
        public string Operation { get; set; }

        public DateTime OpenedDateTime { get; set; }
        public DateTime ClosedDateTime { get; set; }
        public DateTime CanceledDateTime { get; set; }
        public int Status { get; set; }
    }
}
