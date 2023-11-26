using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Orders.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        [Required]
        public string Name { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        [Required]
        public string Unit { get; set; }

    }

    public class OrderItemNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var order = (Order)validationContext.ObjectInstance;
            var orderItems = order.OrderItems;

            if (value != null && order != null && order.OrderItems.Any(oi=>oi.Name == Convert.ToString(value)) )
            {
                return new ValidationResult("Order Number cannot be the same as any of Order Items' Name.");
            }

            return ValidationResult.Success;
        }
    }

}