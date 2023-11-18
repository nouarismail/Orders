using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Orders.Models
{
    public class Order
    {
        public int Id {get; set;}
        [Column(TypeName = "nvarchar(max)")]
        public string? Number {get; set;}
        [Column(TypeName = "datetime2")]
        public DateTime Date{get; set;}
        public int SupplierId{get; set;}

    }
}