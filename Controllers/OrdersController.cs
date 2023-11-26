using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Orders.Models;

namespace Orders.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrdersApplicationDbContext _context;

        public OrdersController(OrdersApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        // public async Task<IActionResult> Index()
        // {
        //     var ordersApplicationDbContext = _context.Order.Include(o => o.Supplier);
        //     return View(await ordersApplicationDbContext.ToListAsync());
        // }
        public async Task<IActionResult> Index(string sortOrder,
        int?[] selectedOrderNumbers,
        int?[] selectedSupplierIds,
        string[] selectedOrderItemIds,
        string hiddenFromDate,
        string hiddenToDate)
        {
            ViewData["NumberSortParm"] = String.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["Suppliers"] = _context.Supplier.ToList();
            var orders = from s in _context.Order.Include(o => o.Supplier)
                         select s;

            if (selectedOrderNumbers != null && selectedOrderNumbers.Length > 0)
            {

                orders = orders.Where(o => selectedOrderNumbers.Contains(o.Id));

            }
            if (selectedSupplierIds != null && selectedSupplierIds.Length > 0)
            {

                orders = orders.Where(o => selectedSupplierIds.Contains(o.SupplierId));

            }
            if (selectedOrderItemIds != null && selectedOrderItemIds.Length > 0)
            {

                orders = orders.Where(o => o.OrderItems.Any(oi => selectedOrderItemIds.Contains(oi.Name)));

            }
            if (!String.IsNullOrEmpty(hiddenToDate) && !String.IsNullOrEmpty(hiddenFromDate))
            {
                DateTime frmDate = DateTime.Parse(hiddenFromDate);
                DateTime tDate = DateTime.Parse(hiddenToDate);
                orders = orders.Where(o => o.Date >= frmDate && o.Date <= tDate);
            }

            switch (sortOrder)
            {
                case "number_desc":
                    orders = orders.OrderByDescending(s => s.Number);
                    break;
                case "Date":
                    orders = orders.OrderBy(o => o.Date);
                    break;
                case "date_desc":
                    orders = orders.OrderByDescending(s => s.Date);
                    break;
                default:
                    orders = orders.OrderBy(s => s.Number);
                    break;
            }


            return View(await orders.ToListAsync());
        }

        public ActionResult GetAutocompleteValues(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<string>());
            }

            var termsArray = term.Split(',').Select(t => t.Trim()).ToArray();
            var orders = _context.Order.ToList();
            var ors = orders.Where(o => termsArray.Any(t => o.Number.Contains(t))).ToList();
            var autocompleteValues = ors
                .Select(order => order.Number)
                .Distinct()
                .ToList();

            return Json(autocompleteValues);
        }

        [HttpGet]
        public JsonResult GetSuppliers(string term)
        {
            // Query the database or any other data source based on the 'term' parameter
            var suppliers = _context.Supplier
                .Where(s => s.Name.Contains(term))
                .Select(s => new { id = s.Id, name = s.Name })
                .ToList();

            return Json(suppliers);
        }

        [HttpGet]
        public JsonResult GetOrderItems(string term)
        {
            // Query the database or any other data source based on the 'term' parameter
            var orderItems = _context.OrderItem
                .Where(oi => oi.Name.Contains(term))
                .Select(oi => new { id = oi.Id, name = oi.Name })
                .ToList();

            return Json(orderItems);
        }

        [HttpGet]
        public JsonResult GetOrderNumbers(string term)
        {
            // Query the database or any other data source based on the 'term' parameter
            var orderNumbers = _context.Order
                .Where(o => o.Number.Contains(term))
                .Select(o => new { id = o.Id, number = o.Number })
                .Distinct()
                .ToList();

            return Json(orderNumbers);
        }

        public ActionResult FilterOrders(string[] selectedOrderNumbers)
        {
            // Use selectedOrderNumbers in your filtering logic
            var filteredOrders = _context.Order
                .Where(o => selectedOrderNumbers.Contains(o.Number))
                .ToList();

            return View("Index", filteredOrders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Supplier)
                .Include(o=>o.OrderItems)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "Name", order.SupplierId);

            return View(order);
            
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "Name");
            Order o = new Order() { OrderItems = new List<OrderItem>() };
            o.OrderItems.Add(new OrderItem());

            return View(o);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,Date,SupplierId,OrderItems")] Order order)
        {
            if (!IsOrderUnique(order.Number, order.SupplierId))
            {
                ModelState.AddModelError("", "Order with the same Number and Provider already exists.");

            }



            if (ModelState.IsValid)
            {
                Order o = new Order() { Number = order.Number, Date = order.Date, SupplierId = order.SupplierId };
                _context.Order.Add(o);
                await _context.SaveChangesAsync();
                foreach (OrderItem oi in order.OrderItems)
                {
                    _context.OrderItem.Add(new OrderItem { Name = oi.Name, OrderId = o.Id, Quantity = oi.Quantity, Unit = oi.Unit });
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "Name", order.SupplierId);
            return View(order);
        }

        private bool IsOrderUnique(string number, int supplierId)
        {
            // Query the database or use Entity Framework to check uniqueness
            return !_context.Order.Any(o => o.Number == number && o.SupplierId == supplierId);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.Include(o => o.OrderItems).FirstOrDefaultAsync(i => i.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "Name", order.SupplierId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Date,SupplierId,OrderItems")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Order orderToUpdate = _context.Order.Find(id);
                    orderToUpdate.SupplierId = order.SupplierId;
                    orderToUpdate.Number = order.Number;
                    orderToUpdate.Date = order.Date;
                    _context.Order.Update(orderToUpdate);

                    Order oldOrder = _context.Order.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == id);
                    List<int> OldOrderItemsIds = oldOrder.OrderItems.Select(oi => oi.Id).ToList();
                    List<int> newOrderItemsIds = order.OrderItems.Select(oi => oi.Id).ToList();

                    foreach (OrderItem oi in oldOrder.OrderItems)
                    {
                        if (newOrderItemsIds.Contains(oi.Id))
                        {
                            //update oi
                            OrderItem newOrderItem = order.OrderItems.FirstOrDefault(item => item.Id == oi.Id);
                            oi.Name = newOrderItem.Name;
                            oi.Unit = newOrderItem.Unit;
                            oi.Quantity = newOrderItem.Quantity;
                            _context.OrderItem.Update(oi);
                        }
                        else
                        {
                            //remove the oi
                            _context.OrderItem.Remove(oi);
                        }
                    }
                    List<OrderItem> newOrderItems = order.OrderItems.Where(oi => oi.Id == -1).ToList();
                    foreach (OrderItem oi in newOrderItems)
                    {
                        _context.OrderItem.Add(new OrderItem { OrderId = order.Id, Name = oi.Name, Quantity = oi.Quantity, Unit = oi.Unit });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "Name", order.SupplierId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'OrdersApplicationDbContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
