using HotelBooking.Interfaces;
using HotelBooking.IServicesLayer;
using HotelBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.ServicesLayer
{
    public class BookingService : IBookingServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        public BookingService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public double CalculateTotalPrice(Booking booking, List<int> roomIds)
        {
            if (booking == null) throw new ArgumentNullException(nameof(booking));
            if (roomIds == null || !roomIds.Any()) return 0;

            int days = (booking.checkOutTime - booking.checkInTime).Days;
            days = Math.Max(days, 1);

            double roomsTotal = roomIds
                .Select(id => _unitOfWork.Rooms.GetById(id))
                .Where(room => room != null)
                .Sum(room => room.PricePerDay * days);

            double servicesTotal = 0;
            if (booking.Gym) servicesTotal += 50;
            if (booking.SPA) servicesTotal += 100;

            return roomsTotal + servicesTotal;
        }

        public void ExpireOldBookings()
        {
            var expiredBookings = _unitOfWork.Bookings.GetAll()
                .Where(b => !b.IsCheckedIn && b.checkInTime < DateTime.Today && !b.IsCheckedOut && !b.IsExpired)
                .ToList();

            foreach (var expired in expiredBookings)
            {
                var rooms = expired.rooms ?? _unitOfWork.Rooms.GetAll()
                                                .Where(r => r.BookingId == expired.Id)
                                                .ToList();

                foreach (var room in rooms)
                {
                    room.IsAvailability = true;
                    room.BookingId = null;
                    _unitOfWork.Rooms.Update(room);
                }

                expired.IsExpired = true;
                _unitOfWork.Bookings.Update(expired);
            }
        }

        public void CreateBooking(Booking booking, List<int> roomIds)
        {
            booking.TotalPrice = CalculateTotalPrice(booking, roomIds);
            _unitOfWork.Bookings.Insert(booking);

            foreach (var roomId in roomIds)
            {
                var room = _unitOfWork.Rooms.GetById(roomId);
                if (room != null)
                {
                    room.BookingId = booking.Id;
                    room.IsAvailability = false;
                    _unitOfWork.Rooms.Update(room);
                }
            }
        }

        public Booking GetByIdWithRooms(int id)
        {
            var booking = _unitOfWork.Bookings.GetAll()
                .FirstOrDefault(b => b.Id == id);

            if (booking != null)
            {
                booking.rooms = _unitOfWork.Rooms.GetAll()
                    .Where(r => r.BookingId == booking.Id)
                    .ToList();

                if (!string.IsNullOrEmpty(booking.ClientId))
                {
                    booking.client = _unitOfWork.Clients.GetById(booking.ClientId);
                    if (booking.client != null)
                    {
                        booking.client.User = _userManager.Users
                            .FirstOrDefault(u => u.Id == booking.client.UserId);
                    }
                }
            }

            return booking;
        }

        public void UpdateBooking(Booking booking, List<int> newRoomIds)
        {
            if (booking == null)
            {
                return;
            }

            var oldBooking = _unitOfWork.Bookings.GetById(booking.Id);
            if (oldBooking == null)
            {
                return;
            }

            var oldRooms = _unitOfWork.Rooms.GetAll().Where(r => r.BookingId == booking.Id).ToList();

            foreach (var room in oldRooms)
            {
                room.IsAvailability = true;
                room.BookingId = null;
                _unitOfWork.Rooms.Update(room);
            }

            oldBooking.checkInTime = booking.checkInTime;
            oldBooking.checkOutTime = booking.checkOutTime;
            oldBooking.Gym = booking.Gym;
            oldBooking.SPA = booking.SPA;
            oldBooking.NumberOfGuests = booking.NumberOfGuests;
            oldBooking.TotalPrice = CalculateTotalPrice(oldBooking, newRoomIds);

            _unitOfWork.Bookings.Update(oldBooking);

            foreach (var roomId in newRoomIds)
            {
                var room = _unitOfWork.Rooms.GetById(roomId);
                if (room != null)
                {
                    room.IsAvailability = false;
                    room.BookingId = oldBooking.Id;
                    _unitOfWork.Rooms.Update(room);
                }
            }
        }
    }

}
