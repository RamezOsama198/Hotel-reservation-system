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
            var comments = _unitOfWork.Comments.GetAll().Select(c => new CommentVM
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                ClientName = _userManager.Users.Where(u => u.Id == c.ClientId).Select(u => u.Name).FirstOrDefault() ?? "Unknown"
            }).ToList();
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
            var roomCount = _unitOfWork.Rooms.GetAll().Count();
            ViewBag.RoomCount = roomCount;
            var bookingCount = _unitOfWork.Bookings.GetAll().Count();
            ViewBag.BookingCount = bookingCount;
            var roomsCountAv = _unitOfWork.Rooms.GetAll().Where(b => b.IsAvailability == true).Count();
            ViewBag.RoomsCountAv = roomsCountAv;
            var comment = _unitOfWork.Comments.GetAll().Count();
            ViewBag.Comment = comment;
            var checkin = _unitOfWork.Bookings.GetAll().Where(b => b.IsCheckedIn == true).Count();
            ViewBag.checkin = checkin;
            var allBookings = _unitOfWork.Bookings.GetAll().Select(b => _bookingService.GetByIdWithRooms(b.Id)).ToList();
            ViewBag.ClientNames = allBookings.ToDictionary(b => b.Id, b => b.client?.User?.Name ?? "Unknown");
            ViewBag.ClientNationalIds = allBookings.ToDictionary(b => b.Id, b => b.client?.NationalId ?? "-");
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

            var staffs = _unitOfWork.Stuffs.GetAll();
            return View(staffs);
        }

        [HttpGet]
        public IActionResult CreateStuff()
        {
            var role = GetAdminRole();
            if (role != "Manager")
                return RedirectToAction("Login", "User");
            var allRooms = _unitOfWork.Rooms.GetAll().ToList();
            if (allRooms == null || !allRooms.Any())
            {
                TempData["ErrorMessage"] = "Cannot create staff because there are no rooms available!";
                return RedirectToAction("GetAllStuffs");
            }
            StuffVM model = new StuffVM
            {
                Rooms = allRooms
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateStuff(StuffVM model)
        {
            var role = GetAdminRole();
            if (role != "Manager")
                return RedirectToAction("Login", "User");
            var allRooms = _unitOfWork.Rooms.GetAll().ToList();

            if (!ModelState.IsValid)
            {
                model.Rooms = allRooms;
                return View(model);
            }

            if (model.SelectedRoomsIds == null || !model.SelectedRoomsIds.Any())
            {
                ModelState.AddModelError("SelectedRoomsIds", "Please select at least one room.");
                model.Rooms = allRooms;
                return View(model);
            }
            var selectedRooms = allRooms.Where(r => model.SelectedRoomsIds.Contains(r.Id)).ToList();

            foreach (var room in selectedRooms)
            {
                _unitOfWork.Rooms.Attach(room); // for many to many
            }
            var stuff = new Stuff
            {
                Name = model.Name,
                Phone = model.Phone,
                Salary = model.Salary,
                JopTitle = model.JopTitle,
                Gender = model.Gender,
                Rooms = selectedRooms
            };
            _unitOfWork.Stuffs.Insert(stuff);
            return RedirectToAction("GetAllStuffs");
        }

        [HttpGet]
        public IActionResult EditStuff(int id)
        {
            var role = GetAdminRole();
            if (role != "Manager")
                return RedirectToAction("Login", "User");

            var stuff = _unitOfWork.Stuffs.GetById(id);
            if (stuff == null) return NotFound();
            var allRooms = _unitOfWork.Rooms.GetAll().ToList();
            stuff.Rooms = _unitOfWork.Rooms.GetRoomsByStuffId(id);
            var model = new StuffVM
            {
                Id = stuff.Id,
                Name = stuff.Name,
                Phone = stuff.Phone,
                Salary = stuff.Salary,
                Gender = stuff.Gender,
                JopTitle= stuff.JopTitle,
                Rooms = allRooms,
                SelectedRoomsIds = stuff.Rooms?.Select(r => r.Id).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditStuff(StuffVM model)
        {
            var role = GetAdminRole();
            if (role != "Manager")
                return RedirectToAction("Login", "User");
            var allRooms = _unitOfWork.Rooms.GetAll().ToList();

            if (!ModelState.IsValid)
            {
                model.Rooms = allRooms;
                return View(model);
            }

            if (model.SelectedRoomsIds == null || !model.SelectedRoomsIds.Any())
            {
                ModelState.AddModelError("SelectedRoomsIds", "Please select at least one room.");
                model.Rooms = allRooms;
                return View(model);
            }

            var stuff = _unitOfWork.Stuffs.GetById(model.Id);
            if (stuff == null) return NotFound();


            stuff.Rooms = _unitOfWork.Rooms.GetRoomsByStuffId(stuff.Id);
            stuff.Rooms.RemoveAll(r => !model.SelectedRoomsIds.Contains(r.Id));

            var selectedRooms = allRooms.Where(r => model.SelectedRoomsIds.Contains(r.Id) && !stuff.Rooms.Any(sr => sr.Id == r.Id)).ToList();

            foreach (var room in selectedRooms)
            {
                stuff.Rooms.Add(room);
            }

            stuff.Name = model.Name;
            stuff.Phone = model.Phone;
            stuff.Salary = model.Salary;
            stuff.Gender = model.Gender;
            stuff.JopTitle = model.JopTitle;


            _unitOfWork.Stuffs.Update(stuff);
            return RedirectToAction("GetAllStuffs");
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
            return RedirectToAction("GetAllStuffs");
        }

        [HttpGet]
        public IActionResult StuffDetails(int id)
        {
            var role = GetAdminRole();
            if (role != "Manager")
                return RedirectToAction("Login", "User");
            var stuff = _unitOfWork.Stuffs.GetById(id);
            stuff.Rooms = _unitOfWork.Rooms.GetRoomsByStuffId(id);
            if (stuff == null) return NotFound();
            return View(stuff);
        }

        //-------------------- Show rooms --------------------
        [HttpGet]
        public IActionResult GetAllRooms()
        {
            var role = GetAdminRole();
            if (role != "Manager")
                return RedirectToAction("Login", "User");

            var rooms = _unitOfWork.Rooms.GetAll();
            return View(rooms);
        }

        [HttpGet]
        public IActionResult RoomDetails(int id)
        {
            var role = GetAdminRole();
            if (role != "Manager")
                return RedirectToAction("Login", "User");
            var room = _unitOfWork.Rooms.GetById(id);
            room.Stuffs = _unitOfWork.Stuffs.GetStuffsByRoomId(id);
            if (room == null) return NotFound();
            return View(room);
        }

        // -------------------- Create Booking for New Client --------------------

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
            if (_unitOfWork.Clients.GetAll().Any(c => c.NationalId == model.NationalId))
            {
                ModelState.AddModelError("NationalId", "This National ID is already registered.");
                ViewBag.AvailableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                return View(model);
            }
            var selectedRooms = _unitOfWork.Rooms.GetAll().Where(r => model.RoomIds.Contains(r.Id)).ToList();
            int totalMaxPeople = selectedRooms.Sum(r => r.MaxPeople);
            if (model.NumberOfGuests > totalMaxPeople)
            {
                ModelState.AddModelError("NumberOfGuests",$"Number of guests cannot exceed {totalMaxPeople}, based on selected rooms.");
                ViewBag.AvailableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                return View(model);
            }
            if (model.CheckInTime > DateTime.Now.AddDays(7))
            {
                ModelState.AddModelError("CheckInTime", "Check-in date cannot be more than 7 days from today.");
                var availableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                ViewBag.AvailableRooms = availableRooms;
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
                client = client,
                ClientId = user.Id,
                checkInTime = model.CheckInTime,
                checkOutTime = model.CheckOutTime,
                Gym = model.Gym,
                SPA = model.SPA,
                IsCheckedIn = false,
                IsCheckedOut = false,
                NumberOfGuests = model.NumberOfGuests
            };

            _bookingService.CreateBooking(booking, model.RoomIds);

            TempData["Success"] = "Client and booking created successfully!";
            return RedirectToAction("Dashboard");
        }

        // -------------------- Create Booking for Old Client --------------------

        [HttpGet]
        public string CheckClientEmail(string nationalId)
        {
            var client = _unitOfWork.Clients.GetByNationalId(nationalId);
            if (client == null)
                return "NotFound";
            if (client.User == null || string.IsNullOrWhiteSpace(client.User.Email))
                return "Email Not Found";
            return client.User.Email ?? "Email Not Found";
        }

        [HttpGet]
        public IActionResult CreateBookingForClient(string nationalId)
        {
            if (string.IsNullOrEmpty(nationalId))
                return RedirectToAction("Dashboard");

            var client = _unitOfWork.Clients.GetByNationalId(nationalId);
            if (client == null)
            {
                TempData["Error"] = "Client not found.";
                return RedirectToAction("Dashboard");
            }

            var model = new ClientBookingVM
            {
                Name = client.User?.Name,
                Email = client.User?.Email,
                NationalId = client.NationalId,
                PhoneNumber = client.User?.PhoneNumber,
                Address = client.Address,
            };

            ViewBag.AvailableRooms = _unitOfWork.Rooms.GetAvailableRooms();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateBookingForClient(ClientBookingVM model)
        {
            var role = GetAdminRole();
            if (role != "Reciption")
                return RedirectToAction("Login", "User");


            if (model.RoomIds == null || !model.RoomIds.Any())
            {
                ModelState.AddModelError("", "You must select at least one room.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                return View(model);
            }
            var client = _unitOfWork.Clients.GetByNationalId(model.NationalId);
            if (client == null)
            {
                ModelState.AddModelError("", "Client not found.");
                ViewBag.AvailableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                return View(model);
            }

            var selectedRooms = _unitOfWork.Rooms.GetAll().Where(r => model.RoomIds.Contains(r.Id)).ToList();
            int totalMaxPeople = selectedRooms.Sum(r => r.MaxPeople);

            if (model.NumberOfGuests > totalMaxPeople)
            {
                ModelState.AddModelError("NumberOfGuests", $"Number of guests cannot exceed {totalMaxPeople}.");
                ViewBag.AvailableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                return View(model);
            }
            if (model.CheckInTime > DateTime.Now.AddDays(7))
            {
                ModelState.AddModelError("CheckInTime", "Check-in date cannot be more than 7 days from today.");
                var availableRooms = _unitOfWork.Rooms.GetAvailableRooms();
                ViewBag.AvailableRooms = availableRooms;
                return View(model);
            }
            var booking = new Booking
            {
                client = client,
                ClientId = client.User?.Id,
                checkInTime = model.CheckInTime,
                checkOutTime = model.CheckOutTime,
                Gym = model.Gym,
                SPA = model.SPA,
                IsCheckedIn = false,
                IsCheckedOut = false,
                NumberOfGuests = model.NumberOfGuests
            };

            _bookingService.CreateBooking(booking, model.RoomIds);
            return RedirectToAction("Dashboard");
        }


    }
}
