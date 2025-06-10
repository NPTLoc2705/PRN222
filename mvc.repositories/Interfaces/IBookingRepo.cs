using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.repositories.Interfaces
{
    public interface IBookingRepo
    {
        List<Booking> GetAllBookings();
        Booking GetBookingById(int id);
        void AddBooking(Booking booking);
        void UpdateBooking(Booking booking);
        void DeleteBookings(int id);
    }
}
