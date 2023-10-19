using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.CompanyVault
{
    public class CompanyVaultCaseDto : IDto
    {
        public decimal BTC { get; set; }
        public decimal USDT { get; set; }
        public decimal TRX { get; set; }
        public decimal ETH { get; set; }

    }
}
