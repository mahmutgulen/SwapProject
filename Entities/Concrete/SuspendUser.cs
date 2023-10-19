using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class SuspendUser : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public bool Status { get; set; }
    }
}
