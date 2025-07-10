using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
using mvc.dataaccess.ViewModels;
using mvc.repositories.Interfaces;

namespace mvc.repositories.Implements
{
    public class BookingRepo : IBookingRepo
    {
        private readonly AppDbContext _context;

        public BookingRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddBookingAsync(Booking booking)
        {
            await _context.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookingsAsync(int id)
        {
            var book = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
            // Ensure that each BookingRepo instance is used for a single operation at a time.
            // DbContext is not thread-safe. Do not share BookingRepo (and thus DbContext) across threads or requests.
            // If using dependency injection, use AddScoped<AppDbContext>() and AddScoped<BookingRepo>() in your DI setup.
            // Example (in Program.cs or Startup.cs):
            // services.AddDbContext<AppDbContext>(options => ...);
            // services.AddScoped<IBookingRepo, BookingRepo>();

            // No code changes are needed in BookingRepo itself, but ensure you do not use the same BookingRepo instance concurrently.
            if (book != null)
            {
                _context.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<Booking?> GetBookingByCustomerIdAsync(Guid id)
        {
            return await _context.Bookings.FirstOrDefaultAsync(c => c.CustomerId.Equals(id));
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<UserBookingRequest>> GetBookingsByCustomer()
        {
            // Filter bookings where Status == 1 (assuming 1 means the desired status)
            var bookings = await _context.Bookings
                .Where(b => (int)b.Status == 1)
                .ToListAsync();

            // Join bookings with users to create UserBookingRequest view models  
            var userBookingRequests = (from booking in bookings
                                       join user in _context.Users on booking.CustomerId equals user.Id
                                       select new UserBookingRequest
                                       {
                                           customerId = user.Id,
                                           UserName = user.FullName,
                                           Email = user.Email,
                                           PhoneNumber = user.PhoneNumber,
                                           BookingDate = booking.BookingDate
                                       }).ToList();

            return userBookingRequests;
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            _context.Update(booking);
            await _context.SaveChangesAsync();
        }
    }
}
