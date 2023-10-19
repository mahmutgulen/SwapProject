using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class CompanyVault : IEntity
    {
        //[Key]
        public int Id { get; set; }
        [Precision(18, 8)]
        public decimal BTC { get; set; }

        [Precision(18, 8)]
        public decimal ETH { get; set; }

        [Precision(18, 8)]
        public decimal TRX { get; set; }
        [Precision(18, 8)]
        public decimal USDT { get; set; }
        public int PendingId { get; set; }
        public int Status { get; set; }
        public DateTime DateTime { get; set; }
    }
}
