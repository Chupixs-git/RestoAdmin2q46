using System;

namespace RestoAdmin.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string BookingNumber { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public TimeSpan BookingTime { get; set; }
        public int DurationHours { get; set; } = 2;
        public int PersonsCount { get; set; }
        public int CustomerId { get; set; }
        public int TableId { get; set; }
        public string? SpecialRequests { get; set; }
        public bool NeedChildChair { get; set; }
        public bool NeedTableDecoration { get; set; }
        public bool NeedFlowers { get; set; }
        public bool NeedCandles { get; set; }
        public bool NeedBalloons { get; set; }
        public bool NeedLiveMusic { get; set; }
        public bool IsBanquet { get; set; }
        public bool IsCorporateEvent { get; set; }
        public bool IsFamilyCelebration { get; set; }
        public bool IsRomanticDinner { get; set; }
        public string Status { get; set; } = "confirmed";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual Customer? Customer { get; set; }
        public virtual Table? Table { get; set; }
    }
}