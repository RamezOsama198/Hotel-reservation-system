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

        [HttpPost]
        public IActionResult AddComment(Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return View(comment);
            }

            _unitOfWork.Comments.Insert(comment);

            TempData["msg"] = "Comment sent successfully!";
            return RedirectToAction("AddComment");
        }
    }
}
