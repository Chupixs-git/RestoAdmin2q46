using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestoAdmin.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PreferredZone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrefersWindowView = table.Column<bool>(type: "bit", nullable: true),
                    PrefersQuietZone = table.Column<bool>(type: "bit", nullable: true),
                    HasRegularChild = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TableZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableZones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableNumber = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HasWindowView = table.Column<bool>(type: "bit", nullable: false),
                    IsStageView = table.Column<bool>(type: "bit", nullable: false),
                    IsVipZone = table.Column<bool>(type: "bit", nullable: false),
                    IsQuietZone = table.Column<bool>(type: "bit", nullable: false),
                    HasChildChair = table.Column<bool>(type: "bit", nullable: false),
                    HasPowerOutlet = table.Column<bool>(type: "bit", nullable: false),
                    NearExit = table.Column<bool>(type: "bit", nullable: false),
                    LightingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NoiseLevel = table.Column<int>(type: "int", nullable: false),
                    XPosition = table.Column<int>(type: "int", nullable: false),
                    YPosition = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tables_TableZones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "TableZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DurationHours = table.Column<int>(type: "int", nullable: false),
                    PersonsCount = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    TableId = table.Column<int>(type: "int", nullable: false),
                    SpecialRequests = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NeedChildChair = table.Column<bool>(type: "bit", nullable: false),
                    NeedTableDecoration = table.Column<bool>(type: "bit", nullable: false),
                    NeedFlowers = table.Column<bool>(type: "bit", nullable: false),
                    NeedCandles = table.Column<bool>(type: "bit", nullable: false),
                    NeedBalloons = table.Column<bool>(type: "bit", nullable: false),
                    NeedLiveMusic = table.Column<bool>(type: "bit", nullable: false),
                    IsBanquet = table.Column<bool>(type: "bit", nullable: false),
                    IsCorporateEvent = table.Column<bool>(type: "bit", nullable: false),
                    IsFamilyCelebration = table.Column<bool>(type: "bit", nullable: false),
                    IsRomanticDinner = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "TableZones",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Столики с панорамным видом на центральную улицу.", "У окна" },
                    { 2, "Столики в непосредственной близости от сцены.", "У сцены" },
                    { 3, "Отдельная зона с мягкими диванами, повышенным уровнем комфорта.", "VIP-зона" },
                    { 4, "Удаленная от сцены зона для спокойного отдыха.", "Тихая зона" }
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "Id", "Capacity", "HasChildChair", "HasPowerOutlet", "HasWindowView", "IsQuietZone", "IsStageView", "IsVipZone", "LightingType", "NearExit", "NoiseLevel", "Status", "TableNumber", "XPosition", "YPosition", "ZoneId" },
                values: new object[,]
                {
                    { 1, 2, false, false, true, false, false, false, "Яркое", false, 3, "free", 1, 50, 50, 1 },
                    { 2, 4, false, false, true, false, false, false, "Яркое", false, 3, "free", 2, 150, 50, 1 },
                    { 3, 2, false, true, true, false, false, false, "Яркое", false, 3, "free", 3, 250, 50, 1 },
                    { 4, 4, true, false, false, false, true, false, "Сценическое", false, 4, "free", 4, 50, 150, 2 },
                    { 5, 6, false, false, false, false, true, false, "Сценическое", false, 4, "free", 5, 150, 150, 2 },
                    { 6, 4, true, false, false, false, true, false, "Сценическое", false, 4, "free", 6, 250, 150, 2 },
                    { 7, 2, false, true, false, true, false, true, "Приглушенное", false, 1, "free", 7, 50, 250, 3 },
                    { 8, 6, false, true, false, true, false, true, "Приглушенное", false, 1, "free", 8, 150, 250, 3 },
                    { 9, 4, true, false, false, true, false, false, "Мягкое", false, 1, "free", 9, 50, 350, 4 },
                    { 10, 6, false, false, false, true, false, false, "Мягкое", false, 1, "free", 10, 150, 350, 4 },
                    { 11, 2, false, false, false, true, false, false, "Мягкое", true, 1, "free", 11, 250, 350, 4 },
                    { 12, 4, true, false, false, true, false, false, "Мягкое", false, 1, "free", 12, 350, 350, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CustomerId",
                table: "Bookings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TableId",
                table: "Bookings",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_ZoneId",
                table: "Tables",
                column: "ZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "TableZones");
        }
    }
}
