using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mvc.repositories.Interfaces;
using mvc.services.Interfaces;

namespace mvc.services.Implements
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepo _bookingRepo;
        public BookingService(IBookingRepo bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }

        public void AddBookingAsync(Booking booking)
        {
            _bookingRepo.AddBooking(booking);
        }

        public void DeleteBookingAsync(int id)
        {
            _bookingRepo.DeleteBookings(id);
        }

        public List<Booking> GetAllBookingsAsync()
        {
            return _bookingRepo.GetAllBookings();
        }

        public Booking GetBookingByIdAsync(int id)
        {
          return _bookingRepo.GetBookingById(id);
        }

        public void UpdateBookingAsync(Booking booking)
        {
            _bookingRepo.UpdateBooking(booking);
        }
    }
}
