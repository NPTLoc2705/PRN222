using System;

public class Booking
{
    public int Id { get; set; }
    public DateTime BookingDate { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ConsultantId { get; set; }
    public DateTime StartDate { get; set; }

    public BookStatus Status { get; set; }

    // Navigation properties (optional based on your ORM setup)
    // public Customer Customer { get; set; }
    // public Consultant Consultant { get; set; }
}

public enum BookStatus
{
    Pending = 1,    // Customer sent request
    Confirmed = 2,  // Consultant confirmed
    Canceled = 3,   // Customer canceled
    Ongoing = 4,    // Consultation ongoing
    Complete = 5    // Consultation complete
}
