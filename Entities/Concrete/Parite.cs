using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Parite : IEntity
    {
        public string symbol { get; set; }
        [Precision(18, 8)]
        public decimal price { get; set; }
    }


}

