using mvc.dataaccess.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public class Booking
{
    public int Id { get; set; }
    public DateTime BookingDate { get; set; }

    public Guid CustomerId { get; set; }
    public Guid ConsultantId { get; set; }
    public DateTime StartDate { get; set; }

    public BookStatus Status { get; set; }
    public string Phone {  get; set; }
}

public enum BookStatus
{
    Pending = 1,    // Customer sent request
    Confirmed = 2,  // Consultant confirmed
    Canceled = 3,   // Customer canceled
    Ongoing = 4,    // Consultation ongoing
    Complete = 5    // Consultation complete
}
