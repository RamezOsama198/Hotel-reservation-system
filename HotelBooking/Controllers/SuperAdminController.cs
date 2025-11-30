using HotelBooking.Interfaces;
using HotelBooking.Models;
using HotelBooking.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "SuperAdmin")]
public class SuperAdminController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;

    public SuperAdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
    }
    public async Task<IActionResult> Dashboard()
    {
        var admins = _unitOfWork.Admins.GetAll().ToList();
        var clients = _unitOfWork.Clients.GetAll().ToList();

        var users = new List<User>();

        foreach (var admin in admins)
        {
            var user = await _userManager.FindByIdAsync(admin.UserId);
            if (user != null)
                users.Add(user);
        }

        foreach (var client in clients)
        {
            var user = await _userManager.FindByIdAsync(client.UserId);
            if (user != null)
                users.Add(user);
        }

        var model = new List<UserWithRoleVM>();
        foreach (var user in users)
        {
            var admin = admins.FirstOrDefault(a => a.UserId == user.Id);
            var roles = await _userManager.GetRolesAsync(user);

            model.Add(new UserWithRoleVM
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = admin != null ? admin.Role : "Client",
                Salary = admin?.Salary
            });
        }

        return View(model);
    }
    // -------------------- Create Admin --------------------
    [HttpGet]
    public IActionResult CreateAdmin()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAdmin(AdminVM model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            Name = model.Name
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(model);
        }

        if (!await _roleManager.RoleExistsAsync("Admin"))
            await _roleManager.CreateAsync(new IdentityRole("Admin"));

        await _userManager.AddToRoleAsync(user, "Admin");

        var admin = new Admin
        {
            UserId = user.Id,
            Role = model.Role,
            Salary = model.Salary
        };

        _unitOfWork.Admins.Insert(admin);

        return RedirectToAction("Dashboard");
    }

    // -------------------- Delete User --------------------
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var admin = _unitOfWork.Admins.GetAll().FirstOrDefault(a => a.UserId == id);
        if (admin != null)
            _unitOfWork.Admins.Delete(admin.UserId);

        var client = _unitOfWork.Clients.GetAll().FirstOrDefault(c => c.UserId == id);
        if (client != null)
            _unitOfWork.Clients.Delete(client.UserId);

        await _userManager.DeleteAsync(user);

        return RedirectToAction("Dashboard");
    }

    // -------------------- Edit User --------------------
    [HttpGet]
    public async Task<IActionResult> EditUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var superAdmin = _unitOfWork.Admins.GetAll().FirstOrDefault(a => a.Role == "SuperAdmin");
        if (superAdmin != null && superAdmin.UserId == id)
            return Forbid();

        var admin = _unitOfWork.Admins.GetAll().FirstOrDefault(a => a.UserId == id);
        var client = _unitOfWork.Clients.GetAll().FirstOrDefault(c => c.UserId == id);

        var model = new UserVM
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = admin != null ? "Admin" : "Client",
            AdminRole = admin?.Role,
            Salary = admin?.Salary,
            Address = client?.Address,
            NationalId = client?.NationalId
        };


        ViewBag.AdminRoles = new List<SelectListItem>
    {
        new SelectListItem { Value = "Reciption", Text = "Reciption" },
        new SelectListItem { Value = "Manager", Text = "Manager" }
    };


        ViewBag.Roles = new List<SelectListItem>
    {
        new SelectListItem { Value = "Client", Text = "Client" },
        new SelectListItem { Value = "Admin", Text = "Admin" }
    };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(string id, UserVM model)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var superAdmin = _unitOfWork.Admins.GetAll().FirstOrDefault(a => a.Role == "SuperAdmin");
        if (superAdmin != null && superAdmin.UserId == id)
            return Forbid();


        if (model.Role == "Client")
        {
            if (string.IsNullOrWhiteSpace(model.Address))
                ModelState.AddModelError("Address", "Address is required for clients.");
            if (string.IsNullOrWhiteSpace(model.NationalId))
                ModelState.AddModelError("NationalId", "National ID is required for clients.");
        }
        
        if (model.Role == "Admin")
        {
            if (string.IsNullOrWhiteSpace(model.AdminRole))
                ModelState.AddModelError("AdminRole", "Admin Role is required for admins.");
            if (model.Salary == null || model.Salary < 1000 || model.Salary > 50000)
                ModelState.AddModelError("Salary", "Salary must be between 1000 and 50000 for admins.");
        }


        if (!ModelState.IsValid)
        {
            ViewBag.AdminRoles = new List<SelectListItem>
        {
            new SelectListItem { Value = "Reciption", Text = "Reciption" },
            new SelectListItem { Value = "Manager", Text = "Manager" }
        };
            ViewBag.Roles = new List<SelectListItem>
        {
            new SelectListItem { Value = "Client", Text = "Client" },
            new SelectListItem { Value = "Admin", Text = "Admin" }
        };
            return View(model);
        }


        user.Name = model.Name;
        user.Email = model.Email;
        user.UserName = model.Email;
        await _userManager.UpdateAsync(user);


        var adminEntity = _unitOfWork.Admins.GetAll().FirstOrDefault(a => a.UserId == id);
        var clientEntity = _unitOfWork.Clients.GetAll().FirstOrDefault(c => c.UserId == id);


        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (model.Role == "Client")
        {

            if (adminEntity != null)
                _unitOfWork.Admins.Delete(adminEntity.UserId);


            if (clientEntity == null)
            {
                clientEntity = new Client
                {
                    UserId = id,
                    Address = model.Address,
                    NationalId = model.NationalId
                };
                _unitOfWork.Clients.Insert(clientEntity);
            }
            else
            {
                clientEntity.Address = model.Address;
                clientEntity.NationalId = model.NationalId;
                _unitOfWork.Clients.Update(clientEntity);
            }

            await _userManager.AddToRoleAsync(user, "Client");
        }
        else 
        {

            if (clientEntity != null)
                _unitOfWork.Clients.Delete(clientEntity.UserId);


            if (adminEntity == null)
            {
                adminEntity = new Admin
                {
                    UserId = id,
                    Role = model.AdminRole,
                    Salary = model.Salary ?? 0
                };
                _unitOfWork.Admins.Insert(adminEntity);
            }
            else
            {
                adminEntity.Role = model.AdminRole;
                adminEntity.Salary = model.Salary ?? 0;
                _unitOfWork.Admins.Update(adminEntity);
            }

            await _userManager.AddToRoleAsync(user, "Admin");
        }

        TempData["Success"] = "User updated successfully!";
        return RedirectToAction("Dashboard");
    }




}