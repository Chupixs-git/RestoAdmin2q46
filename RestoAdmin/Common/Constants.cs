namespace RestoAdmin.Common
{
    public static class TableStatus
    {
        public const string Free = "free";
        public const string Booked = "booked";
        public const string Occupied = "occupied";
    }

    public static class BookingStatus
    {
        public const string Confirmed = "confirmed";
        public const string Cancelled = "cancelled";
    }

    public static class TableColors
    {
        public static readonly string Free = "#00B4DB";
        public static readonly string FreeMatch = "#11998E";
        public static readonly string FreeNoMatch = "#FF6B6B";
        public static readonly string Booked = "#F2994A";
        public static readonly string Occupied = "#EB3349";
        public static readonly string Default = "#BDBDBD";
    }

    public static class ZoneNames
    {
        public const string Window = "У окна";
        public const string Stage = "У сцены";
        public const string Vip = "VIP-зона";
        public const string Quiet = "Тихая зона";
    }
}