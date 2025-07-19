using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mvc.dataaccess.Entities;
using mvc.dataaccess.ViewModels;

namespace mvc.services.Interfaces
{
    public interface IBookingService
    {
        Task AddBookingAsync(Booking booking);
        Task DeleteBookingAsync(int id);
        Task<List<Booking>> GetAllBookingsAsync();
        Task<List<BookingViewModel>> GetAllBookingsWithNamesAsync();
        Task<Booking?> GetBookingByCustomerIdAsync(Guid id);
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<List<UserBookingRequest>> GetBookingsByCustomerIdAsync();
        Task UpdateBookingAsync(Booking booking);
        Task<List<User>> GetConsultantsAsync();
        Task<User?> GetConsultantWithFewestBookingsAsync();
    }
}