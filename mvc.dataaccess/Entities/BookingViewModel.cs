using System;
using mvc.dataaccess.Entities;

namespace mvc.dataaccess.ViewModels
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid ConsultantId { get; set; }
        public string ConsultantName { get; set; }
        public DateTime StartDate { get; set; }
        public BookStatus Status { get; set; }
        public string Phone { get; set; }
    }
}