using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orders.Models
{
    public class OrderViewModel
    {
        public OrderViewModel(){
            Orders = new List<Order>();
            Numbers = new List<string>();
        }
        public List<Order>Orders;
        public List<string> Numbers;
    }
}