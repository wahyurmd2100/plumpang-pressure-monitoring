using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PressMon.Web;
using PressMon.Web.Areas.Identity.Data;
using PressMon.Web.Data;
using PressMon.Web.Views.AppUsers;
using System;
using System.Linq;
using System.Threading.Tasks;
using PressMon.Web.Views.AppUsers;
using TMS.Web.Models;

namespace TMS.Web.Controllers
{
    
    public class AppUsersController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AppUsersController(DataContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoadData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();// Skip number of Rows count  
                var length = Request.Form["length"].FirstOrDefault(); // Paging Length 10,20  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault(); // Sort Column Name  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();// Sort Column Direction (asc, desc)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault(); // Search Value from (Search box)
                int pageSize = length != null ? Convert.ToInt32(length) : 0; //Paging Size (10, 20, 50,100)  
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var allUsers = _userManager.Users.ToList();

                // Get all roles
                var allRoles = _roleManager.Roles.ToList();

                // Join the users and roles based on the roles assigned to each user
                var listUserRole = (from user in allUsers
                                    select new
                                    {
                                        user.Id,
                                        user.FullName,
                                        Role = GetRoles(user),
                                        user.Email,
                                        user.PhoneNumber,
                                    });
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    listUserRole = listUserRole.Where(m => m.FullName.Contains(searchValue) || m.Email.Contains(searchValue) ||
                    m.PhoneNumber.Contains(searchValue));
                }

                //total number of rows counts   
                recordsTotal = listUserRole.Count();
                //Paging   
                var data = listUserRole.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: Banks/AddOrEdit
        [NoDirectAccess]
        public async Task<IActionResult> LoginUser()
        {
            var model = new InputModel();
            model.Username = "";
            model.Password = "";
            return View("~/Views/AppUsers/LoginUser.cshtml", model);
        }
        //POST: Section/AddOrEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginUser(InputModel Input)
        {
            string Message = "";
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    Message = "Login Is Success";
                    return Json(new { IsValid = true, message = Message });
                }
                else
                {

                    Message = "Login Failed";
                }
                return Json(new {IsValid =  false, message = Message});

            }

            // If we got this far, something failed, redisplay form
            return View();
        }
        public async Task<IActionResult> LogOutUser()
        {
            await _signInManager.SignOutAsync();
            return Json(new { IsValid = true });
        }
        // GET: Banks/AddOrEdit
        [Authorize(Roles = "Admin")]
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(string id = null)
        {

            var user = _context.AppUsers.FirstOrDefault(x => x.Id == id);
            populateRoleSection();
            if (user == null)
            {

                return View(new Register());
            }

            else
            {
                var register = new Register();
                register.Id = user.Id;
                register.FullName = user.FullName;
                register.UserName = user.UserName;
                register.Email = user.Email;
                register.Password = "";
                register.ConfirmPassword = "";
                return View(register);
            }
        }
       
        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("Id,FullName,UserName,Email, RoleName,Password,ConfirmPassword,NewPassword")] Register register)
        {
            var UserExist = await _userManager.FindByIdAsync(register.Id);
            if (UserExist == null)
            {
                var newuser = new AppUser { UserName = register.UserName, Email = register.Email, FullName = register.FullName };
                var result = await _userManager.CreateAsync(newuser, register.Password);
                if (result.Succeeded)
                {
                    var user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName == register.UserName);
                    if (user != null)
                    {
                        var roleExists = await _context.Roles.FirstOrDefaultAsync(x => x.Id == register.RoleName);
                        if (roleExists != null)
                        {
                            //Add the role to the user
                            //var RoleOnUser = _context.us
                            var userRole = new UserRole();
                            userRole.UserId = user.Id;
                            userRole.RoleId = roleExists.Id;
                            _context.UserRoles.Add(userRole);
                            await _context.SaveChangesAsync();
                        }
                    }


                    return Json(new { isValid = true, html = Helper.RenderRazorViewString(this, "_ViewAll", _context.AppUsers.ToList()) });
                }
                return Json(new { isValid = false, html = Helper.RenderRazorViewString(this, "AddOrEdit", register) });
            }
            else
            {
                var roleExists = await _context.Roles.FirstOrDefaultAsync(x => x.Id == register.RoleName);

                if (roleExists != null)
                {
                    // Add the role to the user
                    var userRoles = await _context.UserRoles.FirstOrDefaultAsync(u => u.UserId == UserExist.Id);
                    if (userRoles != null)
                    {
                        _context.UserRoles.Remove(userRoles);
                        await _context.SaveChangesAsync();
                    }
                    var userRole = new UserRole();
                    userRole.UserId = UserExist.Id;
                    userRole.RoleId = roleExists.Id;
                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();

                }
                if (register.Password == "" || register.NewPassword == "" || register.Password == null || register.NewPassword == null)
                {
                    return Json(new { isValid = true, html = Helper.RenderRazorViewString(this, "_ViewAll", _context.AppUsers.ToList()) });
                }
                var result = await _userManager.ChangePasswordAsync(UserExist, register.Password, register.NewPassword);

                if (result.Succeeded)
                {
                    return Json(new { isValid = true, html = Helper.RenderRazorViewString(this, "_ViewAll", _context.AppUsers.ToList()) });
                }
                return Json(new { isValid = false, html = Helper.RenderRazorViewString(this, "AddOrEdit", register) });
            }
        }
        private string GetRoles(AppUser user)
        {
            string resultRoles = "";
            var roleUser = _userManager.GetRolesAsync(user).Result;
            if (roleUser.Count > 0)
            {
                resultRoles = roleUser[0].ToString();
            }
            return resultRoles;
        }

        private bool UsernameExists(string username)
        {
            return _context.AppUsers.Any(e => e.UserName == username);
        }
        //get roles
        private void populateRoleSection(object selectedSection = null)
        {
            var AppRoles = (from p in _context.AppRoles select p).ToList();
            var approleip = new AppRole()
            {
                Id = null,
                Name = "--- Select Role ---"
            };
            AppRoles.Insert(0, approleip);
            ViewBag.RolesId = AppRoles;
        }
        private bool IdExists(string id)
        {
            return _context.AppUsers.Any(e => e.Id == id);
        }
    }
}
