using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportCompany.Models;

namespace TransportCompany.Controllers
{
	// poate fi accesat doar de catre Administrator
	[Authorize(Roles = "Admin")]
	public class UsersController : Controller
	{
		private ApplicationDbContext ctx = new ApplicationDbContext();

		// Index -------------------------------------
		[HttpGet]
		public ActionResult Index()
		{
			List<UserViewModel> users = new List<UserViewModel>();

			List<ApplicationUser> UsersList = ctx.Users
			.OrderBy(u => u.UserName)
			.ToList();

			foreach(var user in UsersList)
            {
				UserViewModel uvm = new UserViewModel();
				uvm.User = user;
				IdentityRole userRole = ctx.Roles
				.Find(uvm.User.Roles.First().RoleId);
				uvm.RoleName = userRole.Name;
				users.Add(uvm);
			}

			ViewBag.UsersList = users;

			return View();
		}

		// Details -------------------------------------

		[HttpGet]
		public ActionResult Details(string id)
		{
			if (String.IsNullOrEmpty(id))
			{
				return HttpNotFound("Missing the id parameter!");
			}
			ApplicationUser user = ctx.Users
			.Include("Roles")
			.FirstOrDefault(u => u.Id.Equals(id));
			if (user != null)
			{
				ViewBag.UserRole = ctx.Roles
				.Find(user.Roles.First().RoleId).Name;
				return View(user);
			}
			return HttpNotFound("Cloudn't find the user with given id!");
		}

		// Edit -------------------------------------
		[HttpGet]
		public ActionResult Edit(string id)
		{
			if (String.IsNullOrEmpty(id))
			{
				return HttpNotFound("Missing the id parameter!");
			}
			UserViewModel uvm = new UserViewModel();
			uvm.User = ctx.Users.Find(id);
			IdentityRole userRole = ctx.Roles
			.Find(uvm.User.Roles.First().RoleId);
			uvm.RoleName = userRole.Name;
			return View(uvm);
		}

		[HttpPut]
		public ActionResult Edit(string id, UserViewModel uvm)
		{
			ApplicationUser user = ctx.Users.Find(id);
			try
			{
				if (TryUpdateModel(user))
				{
					var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));
					foreach (var r in ctx.Roles.ToList())
					{
						um.RemoveFromRole(user.Id, r.Name);
					}
					um.AddToRole(user.Id, uvm.RoleName);
					ctx.SaveChanges();
				}
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				return View(uvm);
			}
		}


		// Delete ----------------------------

		[HttpDelete]
		public ActionResult Delete(string id)
		{
			if (id != null)
			{
				ApplicationUser user = ctx.Users.Find(id);

				if (user == null)
				{
					//Masina nu exista in baza de date
					return HttpNotFound("Couldn't find the user with id: " + id + " !");
				}

				ctx.Users.Remove(user);
				ctx.SaveChanges();

				return RedirectToAction("Index");
			}

			//Nu avem parametrul id
			return HttpNotFound("Missing id parameter!");
		}

	}

}