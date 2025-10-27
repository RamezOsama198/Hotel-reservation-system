using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Models
{
    public class HotelDbContext : DbContext
    {
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Stuff> Stuffs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bookings and admin relation many to many
            modelBuilder.Entity<Admin>()
                .HasMany(b => b.Bookings).WithMany(a => a.Admins);

            // Admin an comments
            modelBuilder.Entity<Comment>()
                .HasMany(a => a.Admins).WithMany(c => c.Comments);
            //staff and room
            modelBuilder.Entity<Stuff>()
                .HasMany(r => r.Rooms).WithMany(s => s.Stuffs);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-5KOO9A6\\SQLEXPRESS2022;Database=BookingHotel;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True");
            base.OnConfiguring(optionsBuilder);
        }

    }
}
