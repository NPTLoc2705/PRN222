using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mvc.dataaccess.Entities;
using mvc.dataaccess.ViewModels;

namespace mvc.repositories.Interfaces
{
    public interface IBookingRepo
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<Booking?> GetBookingByCustomerIdAsync(Guid id);
        Task AddBookingAsync(Booking booking);
        Task UpdateBookingAsync(Booking booking);
        Task<List<UserBookingRequest>> GetBookingsByCustomer();
        Task DeleteBookingsAsync(int id);
    
    }
}
