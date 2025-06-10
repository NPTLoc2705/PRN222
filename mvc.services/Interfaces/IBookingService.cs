using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Interfaces
{
    public interface IBookingService
    {
        List<Booking> GetAllBookingsAsync();
        Booking GetBookingByIdAsync(int id);
        void AddBookingAsync(Booking booking);
        void UpdateBookingAsync(Booking booking);
        void DeleteBookingAsync(int id);
    }
}
