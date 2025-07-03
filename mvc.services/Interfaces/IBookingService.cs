using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mvc.dataaccess.Entities;
using mvc.dataaccess.ViewModels;

namespace mvc.services.Interfaces
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task AddBookingAsync(Booking booking);
        Task UpdateBookingAsync(Booking booking);
        Task <List<UserBookingRequest>> GetBookingsByCustomerIdAsync();
        Task DeleteBookingAsync(int id);
    }
}
