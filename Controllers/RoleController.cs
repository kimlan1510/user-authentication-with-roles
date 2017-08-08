using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BasicAuthentication.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BasicAuthentication.Controllers
{  // GET: /Roles/Create
    public class RoleController : Controller
    {
         private readonly ApplicationDbContext _db;
        public RoleController(ApplicationDbContext db)
        {
            _db = db;
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
        public ActionResult Create(string RoleName)
        {
            try
            {
                _db.Roles.Add(new Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole()
                {
                    Name = RoleName
                });
                _db.SaveChanges();
                ViewBag.ResultMessage = "Role created successfully !";
                return RedirectToAction("Index", "ToDo");
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
    }
}
