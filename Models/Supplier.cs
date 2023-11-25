using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Orders.Models
{
    public class Supplier
    {
        public int Id {get; set;}
        [Column(TypeName = "nvarchar(max)")]
        [Required]
        public string Name {get; set;}

        public virtual ICollection<Order> Orders { get; set; }
    }
}