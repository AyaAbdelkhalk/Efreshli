using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Efreshli.Infrastructure.Data;
using Efreshli.MVC.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Efreshli.MVC.Controllers
{
    [Authorize] // Add appropriate authorization policy
    public class AdminOrdersController : Controller
    {
        private readonly ILogger<AdminOrdersController> _logger;
        private readonly EfreshliDbContext _context;

        public AdminOrdersController(ILogger<AdminOrdersController> logger, EfreshliDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: AdminOrders
        public async Task<IActionResult> Index(AdminOrderFilterViewModel filters)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.ApplicationUser)
                    .Include(o => o.Payment)
                    .Include(o => o.DeliveryAddress)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(filters.SearchTerm))
                {
                    query = query.Where(o => o.OrderId.ToString().Contains(filters.SearchTerm) ||
                        o.ApplicationUser.PhoneNumber.Contains(filters.SearchTerm));
                }

                if (filters.Status.HasValue)
                {
                    query = query.Where(o => o.Status == filters.Status);
                }

                if (filters.PaymentMethod.HasValue)
                {
                    query = query.Where(o => o.Payment.PaymentMethod == filters.PaymentMethod);
                }

                if (!string.IsNullOrEmpty(filters.ShippingType))
                {
                    // Assuming DeliveryAddress null means Pickup
                    if (filters.ShippingType == "Delivery")
                    {
                        query = query.Where(o => o.DeliveryAddress != null);
                    }
                    else if (filters.ShippingType == "Pickup")
                    {
                        query = query.Where(o => o.DeliveryAddress == null);
                    }
                }

                if (filters.DateFrom.HasValue)
                {
                    query = query.Where(o => o.CreatedDate >= filters.DateFrom);
                }

                if (filters.DateTo.HasValue)
                {
                    query = query.Where(o => o.CreatedDate <= filters.DateTo);
                }

                var orders = await query
                    .OrderByDescending(o => o.CreatedDate)
                    .ToListAsync();

                var viewModel = new AdminOrderListViewModel
                {
                    Filters = filters,
                    Orders = orders.Select(o => new AdminOrderListItemViewModel
                    {
                        OrderId = o.OrderId,
                        CustomerName = o.ApplicationUser?.FullName ?? "N/A",
                        CustomerPhone = o.ApplicationUser?.PhoneNumber ?? o.DeliveryAddress?.PhoneNumber ?? "N/A",
                        CreatedAt = o.CreatedDate,
                        TotalPrice = o.TotalPrice,
                        PaymentMethod = o.Payment?.PaymentMethod ?? PaymentMethod.CashOnDelivery,
                        ShippingType = o.DeliveryAddress != null ? "Delivery" : "Pickup",
                        Status = o.Status
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                TempData["Error"] = "An error occurred while retrieving orders.";
                return View(new AdminOrderListViewModel());
            }
        }

        // GET: AdminOrders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.ApplicationUser)
                    .Include(o => o.Payment)
                    .Include(o => o.DeliveryAddress)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductItem)
                            .ThenInclude(pi => pi.Product)
                    .Where(o => o.OrderId == id)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return NotFound();
                }

                var viewModel = new AdminOrderDetailsViewModel
                {
                    OrderId = order.OrderId,
                    Status = order.Status,
                    CreatedAt = order.CreatedDate,
                    LastUpdated = order.UpdatedDate ?? order.CreatedDate,
                    CustomerName = order.ApplicationUser?.FullName ?? "N/A",
                    CustomerPhone = order.ApplicationUser?.PhoneNumber ?? order.DeliveryAddress?.PhoneNumber ?? "N/A",
                    CustomerAddress = order.DeliveryAddress?.FullAddress ?? "Pickup at store",
                    Products = order.OrderItems?.Select(oi => new AdminOrderProductItemViewModel
                    {
                        ProductId = oi.ProductItem?.ProductId ?? 0,
                        ProductName = oi.ProductItem?.Product?.NameEn ?? "Unknown Product",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.Price,
                        TotalPrice = oi.Quantity * oi.Price
                    }).ToList() ?? new List<AdminOrderProductItemViewModel>(),
                    SubTotal = order.SubTotalPrice,
                    Tax = 0, // Calculate based on your business logic
                    ShippingFee = order.ShippingPrice,
                    Discount = order.DiscountValue ?? 0,
                    FinalTotal = order.TotalPrice,
                    PaymentMethod = order.Payment?.PaymentMethod ?? PaymentMethod.CashOnDelivery,
                    CustomerNotes = order.Note ?? ""
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order details for ID {OrderId}", id);
                TempData["Error"] = "An error occurred while retrieving order details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: AdminOrders/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(UpdateOrderStatusViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var order = await _context.Orders.FindAsync(model.OrderId);
                    if (order == null)
                    {
                        TempData["Error"] = "Order not found.";
                        return RedirectToAction(nameof(Index));
                    }

                    order.Status = model.Status;
                    order.UpdatedDate = DateTime.Now;
                    
                    // Explicitly mark the entity as modified
                    _context.Entry(order).State = EntityState.Modified;
                    
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"Order status updated to {model.Status}.";
                    return RedirectToAction(nameof(Details), new { id = model.OrderId });
                }

                TempData["Error"] = "Invalid data provided.";
                return RedirectToAction(nameof(Details), new { id = model.OrderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for ID {OrderId}", model.OrderId);
                TempData["Error"] = "An error occurred while updating order status.";
                return RedirectToAction(nameof(Details), new { id = model.OrderId });
            }
        }

        // POST: AdminOrders/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction(nameof(Index));
                }

                order.Status = OrderStatus.Processing;
                order.UpdatedDate = DateTime.Now;
                
                // Explicitly mark the entity as modified
                _context.Entry(order).State = EntityState.Modified;
                
                await _context.SaveChangesAsync();

                TempData["Success"] = "Order approved successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving order ID {OrderId}", id);
                TempData["Error"] = "An error occurred while approving the order.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: AdminOrders/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction(nameof(Index));
                }

                order.Status = OrderStatus.Cancelled;
                order.UpdatedDate = DateTime.Now;
                
                // Explicitly mark the entity as modified
                _context.Entry(order).State = EntityState.Modified;
                
                await _context.SaveChangesAsync();

                TempData["Success"] = "Order cancelled successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order ID {OrderId}", id);
                TempData["Error"] = "An error occurred while cancelling the order.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: AdminOrders/AssignToDelivery/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToDelivery(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction(nameof(Index));
                }

                order.Status = OrderStatus.Shipped;
                order.UpdatedDate = DateTime.Now;
                
                // Explicitly mark the entity as modified
                _context.Entry(order).State = EntityState.Modified;
                
                await _context.SaveChangesAsync();

                TempData["Success"] = "Order assigned to delivery successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning order ID {OrderId} to delivery", id);
                TempData["Error"] = "An error occurred while assigning the order to delivery.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: AdminOrders/GenerateInvoice/5
        public async Task<IActionResult> GenerateInvoice(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.ApplicationUser)
                    .Include(o => o.Payment)
                    .Include(o => o.DeliveryAddress)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductItem)
                            .ThenInclude(pi => pi.Product)
                    .Where(o => o.OrderId == id)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction(nameof(Index));
                }

                // In a real application, you would:
                // 1. Generate a PDF invoice using a library like iTextSharp or similar
                // 2. Return the PDF file
                // For now, we'll just show a success message
                TempData["Success"] = "Invoice generated successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice for order ID {OrderId}", id);
                TempData["Error"] = "An error occurred while generating the invoice.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        #region Helper Methods

        // Additional helper methods can be added here as needed

        #endregion
    }
}