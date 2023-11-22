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
        public async Task<IActionResult> Index()
        {
            var ordersApplicationDbContext = _context.Order.Include(o => o.Supplier);
            return View(await ordersApplicationDbContext.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "Name");
            Order o = new Order(){OrderItems = new List<OrderItem>()};
            o.OrderItems.Add(new OrderItem(){
                Id=1
            });
            
            return View(o);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,Date,SupplierId,OrderItems")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "Name", order.SupplierId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.Include(o=>o.OrderItems).FirstOrDefaultAsync(i => i.Id == id);
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

                    Order oldOrder = _context.Order.Include(o=>o.OrderItems).FirstOrDefault(o=>o.Id==id);
                    List<int>OldOrderItemsIds = oldOrder.OrderItems.Select(oi=>oi.Id).ToList();
                    List<int> newOrderItemsIds = order.OrderItems.Select(oi => oi.Id).ToList();
                    
                    foreach(OrderItem oi in oldOrder.OrderItems)
                    {
                        if(newOrderItemsIds.Contains(oi.Id)){
                            //update oi
                            OrderItem newOrderItem = order.OrderItems.FirstOrDefault(item=>item.Id == oi.Id);
                            oi.Name = newOrderItem.Name;
                            oi.Unit = newOrderItem.Unit;
                            oi.Quantity = newOrderItem.Quantity;
                            _context.OrderItem.Update(oi);
                        }
                        else{
                            //remove the oi
                            _context.OrderItem.Remove(oi);
                        }  
                    }
                    List<OrderItem> newOrderItems = order.OrderItems.Where(oi=>oi.Id==-1).ToList();
                    foreach(OrderItem oi in newOrderItems){
                        _context.OrderItem.Add(new OrderItem{OrderId=order.Id, Name=oi.Name, Quantity=oi.Quantity,Unit=oi.Unit});
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
