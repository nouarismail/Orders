using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Index = Microsoft.EntityFrameworkCore.Metadata.Internal.Index;

namespace Orders.Models
{
    [Index(nameof(Number), nameof(SupplierId), IsUnique = true)]
    public class Order
    {
        public Order()
        {
            OrderItems = new List<OrderItem>();
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "The Number field is required.")]
        [OrderItemName]
        public string Number { get; set; }
        [Column(TypeName = "datetime2")]
        [Required]
        public DateTime Date { get; set; }
        [ForeignKey("Supplier")]
        [Required]
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }



        


        public virtual List<OrderItem> OrderItems { get; set; }

    }
}