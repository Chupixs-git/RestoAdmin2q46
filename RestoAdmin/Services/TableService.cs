using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RestoAdmin.Common;
using RestoAdmin.Database;
using RestoAdmin.Models;

namespace RestoAdmin.Services
{
    public class TableService
    {
        private readonly AppDbContext _context;

        public TableService(AppDbContext context)
        {
            _context = context;
        }

        public List<Table> GetAllTables()
        {
            return _context.Tables.ToList();
        }

        public List<Table> GetFilteredTables(TableFilter filter)
        {
            var query = _context.Tables.AsQueryable();

            if (filter.HasWindowView) query = query.Where(t => t.HasWindowView);
            if (filter.HasStageView) query = query.Where(t => t.IsStageView);
            if (filter.IsVipZone) query = query.Where(t => t.IsVipZone);
            if (filter.IsQuietZone) query = query.Where(t => t.IsQuietZone);
            if (filter.HasChildChair) query = query.Where(t => t.HasChildChair);
            if (filter.HasPowerOutlet) query = query.Where(t => t.HasPowerOutlet);
            if (filter.NearExit) query = query.Where(t => t.NearExit);

            return query.ToList();
        }

        public string GetZoneName(int zoneId)
        {
            var zone = _context.TableZones.FirstOrDefault(z => z.Id == zoneId);
            return zone?.Name ?? "Стандартная зона";
        }

        public string GetStatusText(string status)
        {
            switch (status)
            {
                case TableStatus.Free: return "свободен";
                case TableStatus.Booked: return "забронирован";
                case TableStatus.Occupied: return "занят";
                default: return "неизвестен";
            }
        }

        public bool IsTableAvailable(int tableId, DateTime date, TimeSpan time, int durationHours)
        {
            DateTime startTime = date.Date + time;
            DateTime endTime = startTime.AddHours(durationHours);

            var bookings = _context.Bookings
                .Where(b => b.TableId == tableId && b.Status == BookingStatus.Confirmed)
                .ToList();

            foreach (var b in bookings)
            {
                DateTime bookingStart = b.BookingDate.Date + b.BookingTime;
                DateTime bookingEnd = bookingStart.AddHours(b.DurationHours);

                if (startTime < bookingEnd && endTime > bookingStart)
                {
                    return false;
                }
            }

            return true;
        }

        public void BookTable(Table table, Booking booking)
        {
            table.Status = TableStatus.Booked;
            _context.Entry(table).State = EntityState.Modified;
            _context.Bookings.Add(booking);
            _context.SaveChanges();
        }
    }

    public class TableFilter
    {
        public bool HasWindowView { get; set; }
        public bool HasStageView { get; set; }
        public bool IsVipZone { get; set; }
        public bool IsQuietZone { get; set; }
        public bool HasChildChair { get; set; }
        public bool HasPowerOutlet { get; set; }
        public bool NearExit { get; set; }
    }
}