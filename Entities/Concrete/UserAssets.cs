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
    public class UserAssets : IEntity
    {
       // [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
       
        [Precision(18, 8)]
        public decimal USDT { get; set; }
        [Precision(18, 8)]
        public decimal BTC { get; set; }
        [Precision(18, 8)]
        public decimal ETH { get; set; }
        [Precision(18, 8)]
        public decimal TRX { get; set; }
        public bool status { get; set; }
    }
}
