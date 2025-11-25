
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using HotelBooking.Models;
//using Microsoft.AspNetCore.Authorization;

//namespace HotelBooking.Controllers
//{
//    [Authorize(Roles = "Staff,Admin")]
//    public class StuffsController : Controller
//    {
//        private readonly HotelDbContext _context;

//        public StuffsController(HotelDbContext context)
//        {
//            _context = context;
//        }

//        // Staff Dashboard
//        public IActionResult Index()
//        {
//            var todayBookings = _context.Bookings
//                .Where(b => b.CheckInDate.Date == DateTime.Today)
//                .Include(b => b.User)
//                .Include(b => b.Room)
//                .ToList();

//            return View(todayBookings);
//        }

//        // View all bookings
//        public IActionResult Bookings()
//        {
//            var bookings = _context.Bookings
//                .Include(b => b.client)
//                .Include(b => b.rooms)
//                .ToList();

//            return View(bookings);
//        }
//        // Check-In
//        [HttpPost]
//        public IActionResult CheckIn(int id)
//        {
//            var booking = _context.Bookings.Find(id);
//            if (booking == null) return NotFound();

//            booking.IsCheckedIn = true;
//            _context.SaveChanges();

//            return RedirectToAction("Bookings");
//        }

//        // Check-Out
//        [HttpPost]
//        public IActionResult CheckOut(int id)
//        {
//            var booking = _context.Bookings.Find(id);
//            if (booking == null) return NotFound();

//            booking.IsCheckedOut = true;
//            _context.SaveChanges();

//            return RedirectToAction("Bookings");
//        }

//        // Room Status Page
//        public IActionResult Rooms()
//        {
//            var rooms = _context.Rooms.ToList();
//            return View(rooms);
//        }

//        // Update room status (Clean, Dirty, Maintenance)
//        [HttpPost]
//        public IActionResult UpdateRoomStatus(int roomId, string status)
//        {
//            var room = _context.Rooms.Find(roomId);
//            if (room == null) return NotFound();

//            room.IsAvailability = true;
//            _context.SaveChanges();

//            return RedirectToAction("Rooms");
//        }

//        // Customers Page
//        public IActionResult Customers()
//        {
//            var customers = _context.Users
//                .Where(u => u.GetType == "Client")
//                .ToList();

//            return View(customers);
//        }
//    }
//}