    using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mvc.dataaccess.Entities;
using mvc.dataaccess.ViewModels;
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

        public async Task AddBookingAsync(Booking booking)
        {
            await _bookingRepo.AddBookingAsync(booking);
        }

        public async Task DeleteBookingAsync(int id)
        {
            await _bookingRepo.DeleteBookingsAsync(id);
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepo.GetAllBookingsAsync();
        }

        public async Task<Booking?> GetBookingByCustomerIdAsync(Guid id)
        {
            return await _bookingRepo.GetBookingByCustomerIdAsync(id);
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _bookingRepo.GetBookingByIdAsync(id);
        }

        

        public Task<List<UserBookingRequest>> GetBookingsByCustomerIdAsync()
        {
            return _bookingRepo.GetBookingsByCustomer();
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            await _bookingRepo.UpdateBookingAsync(booking);
        }
    }
}