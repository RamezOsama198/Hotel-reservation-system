using HotelBooking.Interfaces;
using HotelBooking.IServicesLayer;
using HotelBooking.Models;
using HotelBooking.ServicesLayer;
using HotelBooking.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HotelBooking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingServices _bookingService;
        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork, IBookingServices bookingService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _bookingService = bookingService;
        }

        public IActionResult Index()
        {
            var bookings = _unitOfWork.Bookings.GetAll();
            return View(bookings);
        }

        public IActionResult AllComments()
        {
            var comments = _unitOfWork.Comments.GetAll();
            return View(comments);
        }
        public IActionResult Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = _unitOfWork.Admins.GetById(userId);
            ViewBag.AdminRole = admin != null ? admin.Role : "Unknown";

            _bookingService.ExpireOldBookings();
            var stuffCount = _unitOfWork.Stuffs.GetAll().Count();
            ViewBag.StuffCount = stuffCount;
            var RoomCount = _unitOfWork.Rooms.GetAll().Count();
            ViewBag.RoomCount = RoomCount;
            var BookingCount = _unitOfWork.Bookings.GetAll().Count();
            ViewBag.BookingCount = BookingCount;
            var BookingCountAv = _unitOfWork.Rooms.GetAll().Where(b => b.IsAvailability == true).Count();
            ViewBag.BookingCountAv = BookingCountAv;
            var Comment = _unitOfWork.Comments.GetAll().Count();
            ViewBag.Comment = Comment;
            var checkin = _unitOfWork.Bookings.GetAll().Where(b => b.IsCheckedIn == true).Count();
            ViewBag.checkin = checkin;

            var allBookings = _unitOfWork.Bookings.GetAll();
            return View(allBookings);
        }

        [HttpPost]
        public IActionResult CheckIn(int bookingId)
        {
            var booking = _unitOfWork.Bookings.GetById(bookingId);
            if (booking == null) return NotFound();

            booking.IsCheckedIn = true;
            _unitOfWork.Bookings.Update(booking);

            return RedirectToAction("Dashboard", new { id = booking.Id });
        }

        [HttpPost]
        public IActionResult CheckOut(int bookingId)
        {
            var booking = _bookingService.GetByIdWithRooms(bookingId);
            if (booking == null) return NotFound();
            List<int> roomIds = booking.rooms.Select(r => r.Id).ToList();

            booking.checkOutTime = DateTime.Now;
            booking.IsCheckedIn = false;
            booking.IsCheckedOut = true;

            if (booking.rooms != null)
            {
                foreach (var room in booking.rooms)
                {
                    room.IsAvailability = true;
                    room.BookingId = null;
                    _unitOfWork.Rooms.Update(room);
                }
            }
            booking.TotalPrice = _bookingService.CalculateTotalPrice(booking, roomIds);
            _unitOfWork.Bookings.Update(booking);

            return RedirectToAction("Dashboard", new { id = booking.Id });
        }


        //-------------------- CRUD Stuff --------------------

        private string GetAdminRole()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = _unitOfWork.Admins.GetById(userId);

            if (admin == null)
                return null;

            return admin.Role;
        }

        [HttpGet]
        public IActionResult GetAllStuffs()
        {
            var role = GetAdminRole();
            if (role != "Manager")
                return RedirectToAction("Login", "User");

            var staff = _unitOfWork.Stuffs.GetAll();
            return View(staff);
        }

        [HttpGet]
        public IActionResult CreateStuff()
        {
            var role = GetAdminRole();
            if (role != "Manager")
                return RedirectToAction("Login", "User");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateStuff(Stuff model)
        {
            var role = GetAdminRole();
            if (role != "Manager")
                return RedirectToAction("Login", "User");

            if (!ModelState.IsValid) return View(model);

            _unitOfWork.Stuffs.Insert(model);
            return RedirectToAction("GetAllStaff");
        }

        [HttpGet]
        public IActionResult EditStuff(int id)
        {
            var role = GetAdminRole();
            if (role != "Manager")
                return RedirectToAction("Login", "User");

            var staff = _unitOfWork.Stuffs.GetById(id);
            if (staff == null) return NotFound();

            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditStuff(Stuff model)
        {
            var role = GetAdminRole();
            if (role != "Manager")
                return RedirectToAction("Login", "User");

            if (!ModelState.IsValid) return View(model);

            _unitOfWork.Stuffs.Update(model);
            return RedirectToAction("GetAllStaff");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteStuff(int id)
        {
            var role = GetAdminRole();
            if (role != "Manager")
                return RedirectToAction("Login", "User");

            var stuff = _unitOfWork.Stuffs.GetById(id);
            if (stuff == null) return NotFound();

            _unitOfWork.Stuffs.Delete(stuff.Id);
            return RedirectToAction("GetAllStaff");
        }


        [HttpGet]
        public IActionResult CreateBooking()
        {
            var role = GetAdminRole();
            if (role != "Reciption")
                return RedirectToAction("Login", "User");
            ViewBag.AvailableRooms = _unitOfWork.Rooms.GetAvailableRooms();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(ClientBookingVM model)
        {
            var role = GetAdminRole();
            if (role != "Reciption")
                return RedirectToAction("Login", "User");

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                return View(model);
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                ViewBag.AvailableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, "Client");

            var client = new Client
            {
                UserId = user.Id,
                Address = model.Address,
                NationalId = model.NationalId
            };
            _unitOfWork.Clients.Insert(client);

            var booking = new Booking
            {
                ClientId = user.Id,
                checkInTime = model.CheckInTime,
                checkOutTime = model.CheckOutTime,
                Gym = model.Gym,
                SPA = model.SPA,
                IsCheckedIn = false,
                IsCheckedOut = false
            };

            _bookingService.CreateBooking(booking, model.RoomIds);

            TempData["Success"] = "Client and booking created successfully!";
            return RedirectToAction("Dashboard");
        }

        public IActionResult SearchClient()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SearchClient(string nationalId)
        {
            var client = _unitOfWork.Clients.GetByNationalId(nationalId);
            if (client == null)
            {
                ModelState.AddModelError("", "Client not found.");
                return View();
            }

            return RedirectToAction("CreateBookingForClient", new { id = client.UserId });
        }
    }
}
