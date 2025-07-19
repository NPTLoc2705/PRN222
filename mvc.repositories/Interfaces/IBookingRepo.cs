using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mvc.dataaccess.Entities;
using mvc.dataaccess.ViewModels;

namespace mvc.repositories.Interfaces
{
    public interface IBookingRepo
    {
        Task AddBookingAsync(Booking booking);
        Task DeleteBookingsAsync(int id);
        Task<List<Booking>> GetAllBookingsAsync();
        Task<List<BookingViewModel>> GetAllBookingsWithNamesAsync();
        Task<Booking?> GetBookingByCustomerIdAsync(Guid id);
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<List<UserBookingRequest>> GetBookingsByCustomer();
        Task UpdateBookingAsync(Booking booking);
        Task<List<User>> GetConsultantsAsync();
        Task<User?> GetConsultantWithFewestBookingsAsync();
    }
}