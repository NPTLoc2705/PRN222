using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
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

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            _context.Update(booking);
            await _context.SaveChangesAsync();
        }
    }
}
