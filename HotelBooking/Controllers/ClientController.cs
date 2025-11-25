using HotelBooking.Interfaces;
using HotelBooking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public ClientController(HotelDbContext context, UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        //// Client Dashboard (Upcoming bookings)
        //public async Task<IActionResult> Index()
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    var bookings = await _context.Bookings
        //        .Where(b => b.ClientId == user.Id)
        //        .Include(b => b.rooms)
        //        .OrderByDescending(b => b.CheckInDate)
        //        .ToListAsync();

        //    return View(bookings);
        //}

        //// Show available rooms
        //public async Task<IActionResult> Rooms()
        //{
        //    var rooms = await _context.Rooms
        //        .Where(r => r.IsAvailability == true)
        //        .ToListAsync();

        //    return View(rooms);
        //}

        //// GET: Make reservation
        //public async Task<IActionResult> Book(int roomId)
        //{
        //    var room = await _context.Rooms.FindAsync(roomId);
        //    if (room == null) return NotFound();

        //    return View(room);
        //}

        //// POST: Make reservation
        //[HttpPost]
        //public async Task<IActionResult> Book(int roomId, DateTime checkIn, DateTime checkOut)
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    var room = await _context.Rooms.FindAsync(roomId);
        //    if (room == null) return NotFound();

        //    var booking = new Booking
        //    {
        //        RoomId = roomId,
        //        Id = user.Id,
        //        checkInTime = checkIn,
        //        checkOutTime = checkOut,
        //        IsCheckedIn = false,
        //        IsCheckedOut = false
        //    };

        //    _context.Bookings.Add(booking);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Index");
        //}

        //// View my bookings
        //public async Task<IActionResult> MyBookings()
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    var bookings = await _context.Bookings
        //        .Where(b => b.Id == user.Id)
        //        .Include(b => b.Room)
        //        .ToListAsync();

        //    return View(bookings);
        //}

        //// Cancel a reservation
        //[HttpPost]
        //public async Task<IActionResult> CancelBooking(int id)
        //{
        //    var booking = await _context.Bookings.FindAsync(id);
        //    if (booking == null) return NotFound();

        //    var user = await _userManager.GetUserAsync(User);
        //    if (booking.UserId != user.Id) return Unauthorized();

        //    _context.Bookings.Remove(booking);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("MyBookings");
        //}

        //// View profile
        //public async Task<IActionResult> Profile()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    return View(user);
        //}

        //// Update profile
        //[HttpPost]
        //public async Task<IActionResult> Profile(User updatedUser)
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    user.Name = updatedUser.Name;
        //    user.PhoneNumber = updatedUser.PhoneNumber;

        //    await _userManager.UpdateAsync(user);

        //    return RedirectToAction("Profile");
        //}
    }
}
