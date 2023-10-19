using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Entities.Concrete
{
    public class OpenOrder : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Operation { get; set; }
        public string Coin { get; set; }
        [Precision(18, 8)]
        public decimal Amount { get; set; }
        [Precision(18, 8)]
        public decimal Price { get; set; }
        [Precision(18, 8)]
        public decimal Total { get; set; }
        public DateTime DateTime { get; set; }
        public int Status { get; set; }
    }
}
