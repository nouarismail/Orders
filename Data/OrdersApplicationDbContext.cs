using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orders.Models;

namespace Orders.Data
{
    public class OrdersApplicationDbContext : DbContext
    {
        public OrdersApplicationDbContext (DbContextOptions<OrdersApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Orders.Models.Order> Order { get; set; } = default!;

        public DbSet<Orders.Models.OrderItem> OrderItem { get; set; } = default!;

        public DbSet<Orders.Models.Supplier> Supplier { get; set; } = default!;
    }
}
