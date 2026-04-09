using System;
using RestoAdmin.Common;
using RestoAdmin.Database;
using RestoAdmin.Models;

namespace RestoAdmin.Services
{
    public class BookingService
    {
        private readonly AppDbContext _context;
        private static readonly Random _random = new Random();

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public string GenerateBookingNumber()
        {
            return $"АС-{DateTime.Now:ddMMyy}-{_random.Next(1, 99)}";
        }

        public Booking CreateBooking(BookingData data)
        {
            return new Booking
            {
                BookingNumber = GenerateBookingNumber(),
                BookingDate = data.BookingDate,
                BookingTime = data.BookingTime,
                DurationHours = data.DurationHours,
                PersonsCount = data.PersonsCount,
                CustomerId = data.CustomerId,
                TableId = data.TableId,
                SpecialRequests = data.SpecialRequests,
                NeedChildChair = data.NeedChildChair,
                NeedTableDecoration = data.NeedTableDecoration,
                NeedFlowers = data.NeedFlowers,
                NeedCandles = data.NeedCandles,
                NeedBalloons = data.NeedBalloons,
                NeedLiveMusic = data.NeedLiveMusic,
                IsFamilyCelebration = data.IsFamilyCelebration,
                IsRomanticDinner = data.IsRomanticDinner,
                IsCorporateEvent = data.IsCorporateEvent,
                IsBanquet = data.IsBanquet,
                Status = BookingStatus.Confirmed
            };
        }
    }

    public class BookingData
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
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
        public bool IsFamilyCelebration { get; set; }
        public bool IsRomanticDinner { get; set; }
        public bool IsCorporateEvent { get; set; }
        public bool IsBanquet { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan BookingTime { get; set; }
        public int DurationHours { get; set; } = 2;
    }
}