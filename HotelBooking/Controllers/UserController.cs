using HotelBooking.Interfaces;
using HotelBooking.IServicesLayer;
using HotelBooking.Models;
using HotelBooking.ServicesLayer;
using HotelBooking.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBooking.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingServices _bookingService;
        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork, IBookingServices bookingService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _bookingService = bookingService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                    var client = new Client
                    {
                        UserId = user.Id,
                        Address = model.Address,
                        NationalId = model.NationalId
                    };
                    _unitOfWork.Clients.Insert(client);
                return RedirectToAction("RegisterSuccess");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterSuccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                    return RedirectToAction("Dashboard", "Admin");

                if (roles.Contains("Client"))
                    return RedirectToAction("Index", "Client");

                if (roles.Contains("SuperAdmin"))
                    return RedirectToAction("Dashboard", "SuperAdmin");

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["LogoutMessage"] = "You have been logged out.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateBooking(int id)
        {
            var booking = _unitOfWork.Bookings.GetById(id);
            if (booking == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            // if client role, ensure booking belongs to their client record
            if (User.IsInRole("Client"))
            {
                var client = _unitOfWork.Clients.GetAll().FirstOrDefault(c => c.UserId == currentUser.Id);
                if (client == null) return Forbid();

                if (booking.ClientId != client.UserId) return Unauthorized();
            }

            return View(booking);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBooking(Booking model)
        {
            if (!ModelState.IsValid) return View(model);

            var booking = _unitOfWork.Bookings.GetById(model.Id);
            if (booking == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Client"))
            {
                var client = _unitOfWork.Clients.GetAll().FirstOrDefault(c => c.UserId == currentUser.Id);
                if (client == null) return Forbid();

                if (booking.ClientId != client.UserId) return Unauthorized();
            }

            // update allowed fields
            booking.checkInTime = model.checkInTime;
            booking.checkOutTime = model.checkOutTime;
            booking.TotalPrice = model.TotalPrice;
            booking.Gym = model.Gym;
            booking.SPA = model.SPA;

            _unitOfWork.Bookings.Update(booking);

            TempData["Success"] = "Booking updated successfully!";
            // redirect depending on role
            return RedirectToRoleHome();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = _unitOfWork.Bookings.GetById(id);
            if (booking == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Client"))
            {
                var client = _unitOfWork.Clients.GetAll().FirstOrDefault(c => c.UserId == currentUser.Id);
                if (client == null) return Forbid();

                if (booking.ClientId != client.UserId) return Unauthorized();
            }

            _unitOfWork.Bookings.Delete(id);

            TempData["Success"] = "Booking deleted successfully!";

            return RedirectToRoleHome();
        }


        private IActionResult RedirectToRoleHome()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Client");
        }
        //====================================================================================================================//
        [HttpGet]
        public IActionResult ShowAvailableRooms()
        {
            var availableRooms = _unitOfWork.Rooms.GetAvailableRooms();
            if(!availableRooms.Any())
            {
                ViewBag.Message = "No rooms are currently available.";
            }
            return View(availableRooms);
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreateBooking()
        {
            var availableRooms = _unitOfWork.Rooms.GetAvailableRooms();
            ViewBag.AvailableRooms = availableRooms;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(Booking booking, List<int> roomIds)
        {
            _bookingService.ExpireOldBookings();
            if (roomIds == null || !roomIds.Any())
            {
                ModelState.AddModelError("", "You must select at least one room.");
                var availableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                ViewBag.AvailableRooms = availableRooms;
                return View(booking);
            }
            if (!ModelState.IsValid)
            {
                var availableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                ViewBag.AvailableRooms = availableRooms;
                return View(booking);
            }

            

            if (booking.checkInTime > DateTime.Now.AddDays(7))
            {
                ModelState.AddModelError("", "Check-in date cannot be more than 7 days from today.");
                var availableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                ViewBag.AvailableRooms = availableRooms;
                return View(booking);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            if (!isAdmin && roomIds.Count > 5)
            {
                ModelState.AddModelError("", "You cannot book more than 5 rooms at once.");
                var availableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                ViewBag.AvailableRooms = availableRooms;
                return View(booking);
            }

            var client = _unitOfWork.Clients.GetById(currentUser.Id);

            if (client == null)
            {
                ModelState.AddModelError("", "Client profile not found.");
                var availableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                ViewBag.AvailableRooms = availableRooms;
                return View(booking);
            }

            if (isAdmin)
            {
                RedirectToAction("CreateBooking", "Admin");
            }

            booking.ClientId = client.UserId;
            booking.IsCheckedIn = false;
            booking.IsCheckedOut = false;
            
            _bookingService.CreateBooking(booking, roomIds);
            return RedirectToAction("ViewBooking", new { id = booking.Id });
        }

        [Authorize]
        [HttpGet]
        public IActionResult ViewBooking(int id)
        {
            var booking = _unitOfWork.Bookings.GetById(id);
            if (booking == null)
                return NotFound();

            return View(booking);
        }



    }
}
