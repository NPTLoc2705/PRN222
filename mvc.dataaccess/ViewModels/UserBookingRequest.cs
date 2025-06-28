using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.ViewModels
{
    public class UserBookingRequest
    {
        public Guid customerId { get;set;}
        public String UserName { get; set; }
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
