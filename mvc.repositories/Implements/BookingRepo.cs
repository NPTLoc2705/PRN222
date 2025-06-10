using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void AddBooking(Booking booking)
        {
            _context.Add(booking);
            _context.SaveChanges();
        }

        public void DeleteBookings(int id)
        {
           var book= _context.Bookings.FirstOrDefault(b => b.Id == id);
            _context.Remove(book);
        }

        public List<Booking> GetAllBookings()
        {
            return _context.ChangeTracker
                .Entries<Booking>()
                .Select(e => e.Entity)
                .ToList();
        }

        public Booking GetBookingById(int id)
        {
            return _context.Bookings.FirstOrDefault(b => b.Id == id);
        }


        public void UpdateBooking(Booking booking)
        {
            _context.Update(booking);
            _context.SaveChanges();
        }
    }
}
