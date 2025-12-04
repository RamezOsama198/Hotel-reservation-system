using HotelBooking.Interfaces;
using HotelBooking.IServicesLayer;
using HotelBooking.Models;
using HotelBooking.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HotelBooking.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingServices _bookingService;
        public ClientController(UserManager<User> userManager, IUnitOfWork unitOfWork, IBookingServices bookingService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index()
        {

            if (!User.Identity.IsAuthenticated)
            {

                return View();
            }


            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return View();


            var client = _unitOfWork.Clients.GetAll()
                            .FirstOrDefault(c => c.UserId == currentUser.Id);
            if (client == null)
                return View();


            var booking = _unitOfWork.Bookings.GetAll()
                            .FirstOrDefault(b => b.ClientId == client.UserId);


            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> SendComment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendComment(CommentVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            var client = _unitOfWork.Clients.GetAll().FirstOrDefault(c => c.UserId == user.Id);

            if (client == null)
                return Unauthorized();

            var comment = new Comment
            {
                Title = model.Title,
                Description = model.Description,
                ClientId = client.UserId,
            };

            _unitOfWork.Comments.Insert(comment);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var client = _unitOfWork.Clients.GetAll().FirstOrDefault(c => c.UserId == currentUser.Id);
            if (client == null) return Unauthorized();

            var allBookings = _unitOfWork.Bookings.GetAll().Where(b => b.ClientId == client.UserId).Select(b => _bookingService.GetByIdWithRooms(b.Id)).ToList();

            return View(allBookings);
        }

    }
}
