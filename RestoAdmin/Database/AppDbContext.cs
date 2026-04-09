using Microsoft.EntityFrameworkCore;
using RestoAdmin.Common;
using RestoAdmin.Models;

namespace RestoAdmin.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Table> Tables { get; set; }
        public DbSet<TableZone> TableZones { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(@"Server=localhost;Database=AdminStolDB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TableZone>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
            });

            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.TableNumber).IsRequired();
                entity.Property(e => e.Capacity).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.LightingType).HasMaxLength(50);
                entity.HasOne(e => e.Zone)
                    .WithMany(z => z.Tables)
                    .HasForeignKey(e => e.ZoneId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.BookingNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.BookingDate).IsRequired();
                entity.Property(e => e.BookingTime).IsRequired();
                entity.Property(e => e.DurationHours).IsRequired();
                entity.Property(e => e.SpecialRequests).HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Bookings)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Table)
                    .WithMany(t => t.Bookings)
                    .HasForeignKey(e => e.TableId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TableZone>().HasData(
                new TableZone { Id = 1, Name = "У окна", Description = "Столики с панорамным видом" },
                new TableZone { Id = 2, Name = "У сцены", Description = "Столики у сцены" },
                new TableZone { Id = 3, Name = "VIP-зона", Description = "VIP зона" },
                new TableZone { Id = 4, Name = "Тихая зона", Description = "Тихая зона" }
            );

            modelBuilder.Entity<Table>().HasData(
                new Table { Id = 1, TableNumber = 1, Capacity = 2, ZoneId = 1, Status = TableStatus.Free, HasWindowView = true, XPosition = 50, YPosition = 50 },
                new Table { Id = 2, TableNumber = 2, Capacity = 4, ZoneId = 1, Status = TableStatus.Free, HasWindowView = true, XPosition = 150, YPosition = 50 },
                new Table { Id = 3, TableNumber = 3, Capacity = 2, ZoneId = 1, Status = TableStatus.Free, HasWindowView = true, XPosition = 250, YPosition = 50 },
                new Table { Id = 4, TableNumber = 4, Capacity = 4, ZoneId = 2, Status = TableStatus.Free, IsStageView = true, XPosition = 50, YPosition = 150 },
                new Table { Id = 5, TableNumber = 5, Capacity = 6, ZoneId = 2, Status = TableStatus.Free, IsStageView = true, XPosition = 150, YPosition = 150 },
                new Table { Id = 6, TableNumber = 6, Capacity = 4, ZoneId = 2, Status = TableStatus.Free, IsStageView = true, XPosition = 250, YPosition = 150 },
                new Table { Id = 7, TableNumber = 7, Capacity = 2, ZoneId = 3, Status = TableStatus.Free, IsVipZone = true, XPosition = 50, YPosition = 250 },
                new Table { Id = 8, TableNumber = 8, Capacity = 6, ZoneId = 3, Status = TableStatus.Free, IsVipZone = true, XPosition = 150, YPosition = 250 },
                new Table { Id = 9, TableNumber = 9, Capacity = 4, ZoneId = 4, Status = TableStatus.Free, IsQuietZone = true, XPosition = 50, YPosition = 350 },
                new Table { Id = 10, TableNumber = 10, Capacity = 6, ZoneId = 4, Status = TableStatus.Free, IsQuietZone = true, XPosition = 150, YPosition = 350 },
                new Table { Id = 11, TableNumber = 11, Capacity = 2, ZoneId = 4, Status = TableStatus.Free, IsQuietZone = true, XPosition = 250, YPosition = 350 },
                new Table { Id = 12, TableNumber = 12, Capacity = 4, ZoneId = 4, Status = TableStatus.Free, IsQuietZone = true, XPosition = 350, YPosition = 350 }
            );
        }
    }
}