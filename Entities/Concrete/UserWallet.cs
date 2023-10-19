using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Entities.Concrete
{
    public class UserWallet : IEntity
    {
        [Key]
        public int UserId { get; set; }
        [Precision(18,8)]
        public decimal Balance { get; set; }
        
    }
}
