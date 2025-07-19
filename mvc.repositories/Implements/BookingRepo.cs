using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<BookingViewModel>> GetAllBookingsWithNamesAsync()
        {
            var query = from booking in _context.Bookings
                        join customer in _context.Users on booking.CustomerId equals customer.Id into customerGroup
                        from customer in customerGroup.DefaultIfEmpty()
                        join consultant in _context.Users on booking.ConsultantId equals consultant.Id into consultantGroup
                        from consultant in consultantGroup.DefaultIfEmpty()
                        select new BookingViewModel
                        {
                            Id = booking.Id,
                            BookingDate = booking.BookingDate,
                            CustomerId = booking.CustomerId,
                            CustomerName = customer != null ? customer.FullName : "Unknown",
                            ConsultantId = booking.ConsultantId,
                            ConsultantName = consultant != null ? consultant.FullName : "Unknown",
                            StartDate = booking.StartDate,
                            Status = booking.Status,
                            Phone = booking.Phone
                        };

            var result = await query.ToListAsync();
            System.Diagnostics.Debug.WriteLine($"Retrieved {result.Count} bookings with names.");
            return result;
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
            var bookings = await _context.Bookings.ToListAsync();

            var userBookingRequests = (from booking in bookings
                                       join user in _context.Users on booking.CustomerId equals user.Id
                                       select new UserBookingRequest
                                       {
                                           customerId = user.Id,
                                           UserName = user.FullName,
                                           Email = user.Email,
                                           PhoneNumber = user.PhoneNumber,
                                           BookingDate = booking.StartDate,
                                           Status = booking.Status
                                       }).ToList();

            return userBookingRequests;
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            _context.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetConsultantsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == SystemRole.Consultant && u.IsActive)
                .ToListAsync();
        }

        public async Task<User?> GetConsultantWithFewestBookingsAsync()
        {
            var activeStatuses = new[] { BookStatus.Pending, BookStatus.Confirmed, BookStatus.Ongoing };
            var consultant = await _context.Users
                .Where(u => u.Role == SystemRole.Consultant && u.IsActive)
                .GroupJoin(_context.Bookings
                    .Where(b => activeStatuses.Contains(b.Status)),
                    u => u.Id,
                    b => b.ConsultantId,
                    (u, b) => new { User = u, BookingCount = b.Count() })
                .OrderBy(x => x.BookingCount)
                .Select(x => x.User)
                .FirstOrDefaultAsync();

            if (consultant == null)
            {
                System.Diagnostics.Debug.WriteLine("No active consultants found.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Selected consultant: {consultant.FullName} with ID: {consultant.Id}");
            }

            return consultant;
        }
    }
}