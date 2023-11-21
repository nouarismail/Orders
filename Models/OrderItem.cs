using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Orders.Models
{
    public class OrderItem
    {
        public int Id {get; set;}
        [ForeignKey("Order")]
        public int OrderId{get; set;}
        public Order? Order {get; set;}
        [Column(TypeName = "nvarchar(max)")]
        public string? Name {get; set;}
        [Column(TypeName = "decimal(18, 3)")]
        public decimal Quantity{get; set;}
        [Column(TypeName = "nvarchar(max)")]
        public string? Unit {get; set;}

    }
}