using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using PressMon.Web.Data;
using PressMon.Web.Models;
using PressMon.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace PressMon.Web.Controllers
{
    public class AppUsersController : Controller
    {
        private readonly TMSContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AppUsersController(TMSContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
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

                // getting all data  
                var appUsers = (from a in _context.AppUsers
                                select a);
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    appUsers = appUsers.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    appUsers = appUsers.Where(m => m.FullName.Contains(searchValue) || m.Email.Contains(searchValue) ||
                    m.PhoneNumber.Contains(searchValue));
                }

                //total number of rows counts   
                recordsTotal = appUsers.Count();
                //Paging   
                var data = appUsers.Skip(skip).Take(pageSize).ToList();
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
        public async Task<IActionResult> AddOrEdit(string id = "")
        {
            if (id == "" || id == null)//flagged as insert
            {
                UserRole usr = new UserRole();
                usr.ApplicationUser = new AppUser();
                usr.ApplicationRoles = await _roleManager.Roles.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id
                }).ToListAsync();
                return View(usr);
            }
            else
            {
                UserRole userRole = new UserRole();
                var appUser = _context.Users.Where(x => x.Id.Equals(id)).SingleOrDefault();
                var userInRole = _context.UserRoles.Where(x => x.UserId.Equals(id)).Select(x => x.RoleId).ToList();
                userRole.ApplicationUser = appUser;
                userRole.ApplicationRoles = await _roleManager.Roles.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id,
                    Selected = userInRole.Contains(x.Id)
                }).ToListAsync();
                
                if (appUser == null)
                {
                    return NotFound();
                }
                return View(userRole);
            }
        }

        // POST: AppUsers/AddOrEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddOrEdit(string id, [Bind("UserName,Email,PhoneNumber,FullName,BirthPlace,BirthDate,EmployeeNumber,StartJoinDate,EndJoinDate,Address,SectionId,DepartmentId,DivisionId,JobId,GradeId,BankId")] UserRole appUser)
        public async Task<IActionResult> AddOrEdit(string id, UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                if (IdExists(id)) //update ini km dapat dari mana man?
                    //itu udah ada dari sana nyya mas, saya beelu edit
                {
                    try
                    {
                        var usr = await _userManager.FindByIdAsync(id);
                        var selectedRoleId = userRole.ApplicationRoles.Where(x => x.Selected).Select(x => x.Value);
                        var alreadyExists = _context.UserRoles.Where(x => x.UserId.Equals(id)).Select(x => x.RoleId).ToList();
                        var toAdd = selectedRoleId.Except(alreadyExists);
                        var toRemove = alreadyExists.Except(selectedRoleId);

                        //update app users
                        usr.FullName = userRole.ApplicationUser.FullName;
                        usr.UserName = userRole.ApplicationUser.UserName;
                        usr.Email = userRole.ApplicationUser.Email;
                        usr.PhoneNumber = userRole.ApplicationUser.PhoneNumber;

                        foreach (var item in toRemove)
                        {
                            _context.UserRoles.Remove(new IdentityUserRole<string>
                            {
                                RoleId = item,
                                UserId = usr.Id
                            });
                        }
                        foreach (var item in toAdd)
                        {
                            _context.UserRoles.Add(new IdentityUserRole<string>
                            {
                                RoleId = item,
                                UserId = usr.Id
                            });
                        }

                        var update = await _userManager.UpdateAsync(usr);

                        if (!update.Succeeded)
                        {
                            foreach (var error in update.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UsernameExists(userRole.ApplicationUser.UserName))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else //create new
                {
                    userRole.ApplicationUser.EmailConfirmed = true;
                    var selectedRoleId = userRole.ApplicationRoles.Where(x => x.Selected).Select(x => x.Value);

                    foreach (var item in selectedRoleId)
                    {
                        _context.UserRoles.Add(new IdentityUserRole<string>
                        {
                            RoleId = item,
                            UserId = userRole.ApplicationUser.Id
                        });
                    }

                    var result = await _userManager.CreateAsync(userRole.ApplicationUser, userRole.ApplicationUser.UserName);

                    if (result.Succeeded)
                    {
                        return Json(new { isValid = true, html = Helper.RenderRazorViewString(this, "_ViewAll", _context.AppUsers.ToList()) });
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return Json(new { isValid = false, html = Helper.RenderRazorViewString(this, "AddOrEdit", userRole.ApplicationUser) });
                    }
                }
                return Json(new { isValid = true, html = Helper.RenderRazorViewString(this, "_ViewAll", _context.AppUsers.ToList()) });
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return Json(new { isValid = false, html = Helper.RenderRazorViewString(this, "AddOrEdit", userRole) });
            }
        }

        private bool UsernameExists(string username)
        {
            return _context.AppUsers.Any(e => e.UserName == username);
        }

        private bool IdExists(string id)
        {
            return _context.AppUsers.Any(e => e.Id == id);
        }
    }
}
