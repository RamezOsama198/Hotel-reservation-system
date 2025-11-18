using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Models
{
    public class HotelDbContext : IdentityDbContext<User>
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Stuff> Stuffs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1-to-1 with Identity User
            modelBuilder.Entity<Client>()
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Client>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne()
                .HasForeignKey<Admin>(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-many
            modelBuilder.Entity<Admin>()
                .HasMany(b => b.Bookings)
                .WithMany(a => a.Admins);

            modelBuilder.Entity<Comment>()
                .HasMany(a => a.Admins)
                .WithMany(c => c.Comments);

            modelBuilder.Entity<Stuff>()
                .HasMany(r => r.Rooms)
                .WithMany(s => s.Stuffs);
        }
    }
}
