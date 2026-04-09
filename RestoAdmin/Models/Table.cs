using System.Collections.Generic;

namespace RestoAdmin.Models
{
    public class Table
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public int ZoneId { get; set; }
        public string Status { get; set; } = "free";
        public bool HasWindowView { get; set; }
        public bool IsStageView { get; set; }
        public bool IsVipZone { get; set; }
        public bool IsQuietZone { get; set; }
        public bool HasChildChair { get; set; }
        public bool HasPowerOutlet { get; set; }
        public bool NearExit { get; set; }
        public string? LightingType { get; set; }
        public int NoiseLevel { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public virtual TableZone? Zone { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}