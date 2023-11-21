using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Orders.Models.ViewModels
{
    public class OrderViewModel
    {
        public List<SelectListItem>? Suppliers { set;get;}

        
    }
}