using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BasicAuthentication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;




// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BasicAuthentication.Controllers
{  // GET: /Roles/Create
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public RoleController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            
        }
        public ActionResult Index()
        {
            var roles = _db.Roles.ToList();
            return View(roles);
        }
        //Create a role
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public IActionResult Create(string RoleName)
        {
            try
            {
                _db.Roles.Add(new IdentityRole()
                {
                    Name = RoleName,
                    NormalizedName = RoleName.ToUpper()
                });

                _db.SaveChanges();
                ViewBag.ResultMessage = "Role created successfully !";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        //delete a role
        public ActionResult Delete(string RoleName)
        {
            var thisRole = _db.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            _db.Roles.Remove(thisRole);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: /Roles/Edit/
        public ActionResult Edit(string roleName)
        {
            var thisRole = _db.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            return View(thisRole);
        }

        //
        // POST: /Roles/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole role)
        {
            var newRole = _db.Roles.Where(r => r.Id == role.Id).FirstOrDefault();
            newRole.Name = role.Name;
            _db.Entry(newRole).State = EntityState.Modified;
            _db.SaveChanges();

            return RedirectToAction("Index");

        }
        public ActionResult ManageUserRoles()
        {
            // prepopulat roles for the view dropdown
            var list = _db.Roles.OrderBy(r => r.Name).ToList().Select(rr =>

            new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            ViewBag.UserName = new SelectList(_db.Users, "UserName", "UserName");
            return View();
        }
        //Add a user to a role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleAddToUser(string UserName, string Roles)
        {
            
            ApplicationUser user = await _userManager.FindByNameAsync(UserName);
            await _userManager.AddToRoleAsync(user, Roles);
            
            ViewBag.ResultMessage = "Role created successfully !";
            // prepopulat roles for the view dropdown
            var list = _db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            ViewBag.UserName = new SelectList(_db.Users, "UserName", "UserName");

            return View("ManageUserRoles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetRoles(string UserName)
        {
            if(!string.IsNullOrWhiteSpace(UserName))
            {
                ApplicationUser user = await _userManager.FindByNameAsync(UserName);
                //var account = new AccountController(_userManager, _signInManager, _db);
                ViewBag.RolesForThisUser = await _userManager.GetRolesAsync(user);
                // prepopulat roles for the view dropdown
                var list = _db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
                ViewBag.UserName = new SelectList(_db.Users, "UserName", "UserName");
            }
            return View("ManageUserRoles");
        }
    }
}