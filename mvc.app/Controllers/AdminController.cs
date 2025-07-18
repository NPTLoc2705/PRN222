using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
using mvc.services.Implements;
using mvc.services.Interfaces;

namespace mvc.app.Controllers
{
    public class AdminController : Controller
    {
        private readonly IBookingService _bookingService;
        public AdminController(IBookingService bookingService, IAuthService userService)
        {
            _bookingService = bookingService;
        }
        public async Task<IActionResult> Dashboard()
        {
            var role = HttpContext.Session.GetString("Role");            // This action could be used to display admin dashboard information
            var customerIdObj = HttpContext.Session.GetString("UserId");
            if (customerIdObj == null || role == "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }
            var list = await _bookingService.GetAllBookingsWithNamesAsync();
            return View("AdminPage",list);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _bookingService.GetBookingByIdAsync(id.Value);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookingDate,CustomerId,ConsultantId,StartDate,Status,Phone")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _bookingService.UpdateBookingAsync(booking);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
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
            return View(booking);
        }
        private bool BookingExists(int id)
        {
            var booking = _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
