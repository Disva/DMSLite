using DMSLite.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMSLite.Controllers
{
    [Authorize(Roles = "SPS")]
    public class RolesController : Controller
    {
        ApplicationDbContext context;
        UserManager<ApplicationUser> userManager;

        public RolesController()
        {
            context = new ApplicationDbContext();

            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        }

        public ActionResult Index()
        {
            var roles = context.Roles.ToList();

            var message = TempData["ResultMessage"];
            TempData["ResultMessage"] = string.Empty;
            ViewBag.ResultMessage = message;

            return View(roles);
        }

        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = collection["RoleName"]
                });
                context.SaveChanges();
                ViewBag.ResultMessage = "Role created successfully !";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult SetRoles()
        {
            ViewBag.Roles = context.Roles.ToDictionary(k => k.Id, v => v.Name);
            return View(context.Users.ToList());
        }

        public ActionResult MakeSPS(string userId)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            userManager.AddToRole(userId, "SPS");

            TempData["ResultMessage"] = "User added to role";
            return RedirectToAction("Index");
        }

        public ActionResult RemoveSPS(string userId)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (userManager.IsInRole(userId, "SPS"))
            {
                userManager.RemoveFromRole(userId, "SPS");
                TempData["ResultMessage"] = "User removed from role";
            }
            else
            {
                TempData["ResultMessage"] = "User was not a silent partner";
            }

            return RedirectToAction("Index");
        }

        public ActionResult MakeAdmin(string userId)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            userManager.AddToRole(userId, "Admin");

            TempData["ResultMessage"] = "User added to role";
            return RedirectToAction("Index");
        }

        public ActionResult RemoveAdmin(string userId)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (userManager.IsInRole(userId, "Admin"))
            {
                userManager.RemoveFromRole(userId, "Admin");
                TempData["ResultMessage"] = "User removed from role";
            }
            else
            {
                TempData["ResultMessage"] = "User was not an admin";
            }

            return RedirectToAction("Index");
        }
    }
}