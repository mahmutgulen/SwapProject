using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class PagingModel
    {
        public PagingModel()
        {
            Users = new List<User>();
            OpenOrders = new List<OpenOrder>();
        }
        public int ActivePage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPageCount { get; set; }
        public List<User> Users { get; set; }
        public List<OpenOrder> OpenOrders { get; set; }
    }
}
