using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mvc.dataaccess.Entities;
namespace mvc.dataaccess.ViewModels
{
    public class UserBookingRequest
    {
        public Guid customerId { get;set;}
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public BookStatus Status { get; set; }
    }

}
