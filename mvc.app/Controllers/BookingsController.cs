using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
using mvc.dataaccess.ViewModels;
using mvc.services.Interfaces;

namespace mvc.app.Controllers
{
    public class BookingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBookingService _bookingService;
        private readonly IAuthService _userService;

        public BookingsController(AppDbContext context, IBookingService bookingService, IAuthService userService)
        {
            _context = context;
            _bookingService = bookingService;
            _userService = userService;
        }

        public async Task<IActionResult> Booking()
        {
            var customerIdObj = HttpContext.Session.GetString("UserId");
            if (customerIdObj == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        public async Task<IActionResult> ConsultanView()
        {
            var userBookingRequests = await _bookingService.GetBookingsByCustomerIdAsync();
            return View(userBookingRequests);
        }

        public async Task<IActionResult> Index()
        {
            var customerIdObj = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (customerIdObj == null || role == "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }
            var list = await _bookingService.GetAllBookingsWithNamesAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookingService.GetBookingByIdAsync(id);
            return View(book);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult UserBooking()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookingDate,CustomerId,ConsultantId,StartDate,Status,Phone")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.Status = BookStatus.Ongoing;
                await _bookingService.AddBookingAsync(booking);
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserBooking([Bind("StartDate,Phone")] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                System.Diagnostics.Debug.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                return View(booking);
            }

            var customerIdObj = HttpContext.Session.GetString("UserId");
            if (customerIdObj == null || !Guid.TryParse(customerIdObj, out var customerId))
            {
                System.Diagnostics.Debug.WriteLine("Session UserId is null or invalid.");
                return RedirectToAction("Login", "Auth");
            }

            var consultant = await _bookingService.GetConsultantWithFewestBookingsAsync();
            if (consultant == null)
            {
                ModelState.AddModelError("", "No available consultants found.");
                return View(booking);
            }

            booking.CustomerId = customerId;
            booking.BookingDate = DateTime.Now;
            booking.Status = BookStatus.Pending;
            booking.ConsultantId = consultant.Id;

            try
            {
                await _bookingService.AddBookingAsync(booking);
                System.Diagnostics.Debug.WriteLine($"Booking created for CustomerId: {customerId}, ConsultantId: {consultant.Id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving booking: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the booking.");
                return View(booking);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> BookConsultant(Guid customerId)
        {
            var consultantIdStr = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (consultantIdStr == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var booking = await _bookingService.GetBookingByCustomerIdAsync(customerId);
            if (booking == null)
            {
                return Json(new { success = false, message = "Booking not found" });
            }
            booking.Status = BookStatus.Ongoing;
            booking.ConsultantId = Guid.Parse(consultantIdStr);
            await _bookingService.UpdateBookingAsync(booking);

            return View("ConsultanView");
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

        public async Task<IActionResult> Delete(int? id)
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookingService.DeleteBookingAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBookingStatus(Guid customerId, [Bind("Status")] Booking booking)
        {
            var consultantIdStr = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (consultantIdStr == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var existingBooking = await _bookingService.GetBookingByCustomerIdAsync(customerId);
            if (existingBooking == null)
            {
                return Json(new { success = false, message = "Booking not found" });
            }

            var validTransitions = new Dictionary<BookStatus, List<BookStatus>>
            {
                { BookStatus.Pending, new List<BookStatus> { BookStatus.Ongoing } },
                { BookStatus.Ongoing, new List<BookStatus> { BookStatus.Complete } }
            };

            if (!validTransitions.ContainsKey(existingBooking.Status) ||
                !validTransitions[existingBooking.Status].Contains(booking.Status))
            {
                return Json(new { success = false, message = "Invalid status transition" });
            }

            existingBooking.Status = booking.Status;
            existingBooking.ConsultantId = Guid.Parse(consultantIdStr);
            await _bookingService.UpdateBookingAsync(existingBooking);

            return Json(new { success = true, message = "Status updated successfully" });
        }
    }
}