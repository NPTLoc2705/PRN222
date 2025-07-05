using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
using mvc.services.Interfaces;

namespace mvc.app.Controllers
{
    public class BookingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBookingService _bookingService;
        private readonly IAuthService _userService;

        public BookingsController(AppDbContext context,IBookingService bookingService, IAuthService service)
        {
            _context = context;
            _bookingService = bookingService;
            _userService = service;
        }
        public async Task<IActionResult> Booking()
        {
            var customerIdObj = HttpContext.Session.GetString("UserId");
            if (customerIdObj == null)
            {
                // Handle missing session (e.g., redirect to login)
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }
        public async Task<IActionResult> ConsultanView()
        {
            var userBookingRequests=await _bookingService.GetBookingsByCustomerIdAsync();

            return View(userBookingRequests);
        }

        
        // GET: Bookings

        public async Task<IActionResult> Index()
        {
            var customerIdObj = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (customerIdObj == null|| role == "Admin")
            {
                // Handle missing session (e.g., redirect to login)
                return RedirectToAction("Login", "Auth");
            }
                var list = await _bookingService.GetAllBookingsAsync();
            return View(list);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookingService.GetBookingByIdAsync(id);
            return View(book);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> UserBooking()
        {   
            return View();
        }



        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookingDate,CustomerId,ConsultantId,StartDate,Status")] Booking booking)
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
        public async Task<IActionResult> UserBooking([Bind("Id,ConsultantId,StartDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Retrieve CustomerId from session
                var customerIdObj = HttpContext.Session.GetString("UserId");
                if (customerIdObj == null)
                {
                    // Handle missing session (e.g., redirect to login)
                    return RedirectToAction("Login", "Auth");
                }
                booking.CustomerId = Guid.Parse(customerIdObj);
                booking.BookingDate = DateTime.Now; // Set the booking date to now
                booking.Status = BookStatus.Pending; // Set the initial status to Pending
                await _bookingService.AddBookingAsync(booking);
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }
        /*  // GET: Bookings/Edit/5
          public async Task<IActionResult> Book(Guid id)
          {
              if (id == null )
              {
                  return NotFound();
              }
              var consultanId = HttpContext.Session.GetString("UserId");
              var role = HttpContext.Session.GetString("Role");
              if (consultanId == null || role == "Consultan")
              {
                  // Handle missing session (e.g., redirect to login)
                  return RedirectToAction("Login", "Auth");
              }
              var booking = await _bookingService.GetBookingByCustomerIdAsync(id);
              if (booking == null)
              {
                  return NotFound();
              }

              // Set the ConsultantId for confirmation
              booking.ConsultantId = Guid.Parse(consultanId);

              return View(booking); // Create a Book.cshtml view or reuse Edit.cshtml
          }*/
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(Guid id, [Bind("Id,")] Booking booking)
        {
            if (id != booking.CustomerId)
            {
                return NotFound();
            }

            var existingBooking = await _bookingService.GetBookingByCustomerIdAsync(id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            existingBooking.ConsultantId = booking.ConsultantId;
            await _bookingService.UpdateBookingAsync(existingBooking);

            return RedirectToAction(nameof(Index));
        }*/
        
        public async Task<IActionResult> BookConsultant(Guid customerId)
        {
            var consultantIdStr = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (consultantIdStr == null /*|| role != "Consultan"*/)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var booking = await _bookingService.GetBookingByCustomerIdAsync(customerId);
            if (booking == null)
            {
                return Json(new { success = false, message = "Booking not found" });
            }
            booking.Status = BookStatus.Ongoing; // Update the status to Ongoing
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

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookingDate,CustomerId,ConsultantId,StartDate,Status")] Booking booking)
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

        // GET: Bookings/Delete/5
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

        // POST: Bookings/Delete/5
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
    }
}
