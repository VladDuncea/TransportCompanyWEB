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
    public class TransportController : Controller
    {
		private ApplicationDbContext ctx = new ApplicationDbContext();

		// Index -----------------------------
		[HttpGet]
		[Authorize(Roles = "Admin,Driver")]
		public ActionResult Index()
		{
			List<Transport> transports;
			if (User.IsInRole("Admin"))
			{
				//Afisam toate transporturile
				transports = ctx.Transports.ToList();
			}
			else
			{
				//Soferul curent
				ApplicationUser driver = ctx.Users.Find(User.Identity.GetUserId());
				//Daca utilizatorul curent e sofer luam doar traseele lui
				transports = ctx.Transports.Where(transport => transport.Driver.Id == driver.Id).ToList();
			}

			ViewBag.Transports = transports;

			return View();
		}

		// Details ----------------------------
		[HttpGet]
		[Authorize(Roles = "Admin,Driver")]
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return HttpNotFound("Missing id parameter!");
			}

			//Cautam transport
			Transport transport = ctx.Transports.Find(id);

			//Verificare existenta transport
			if (transport == null)
			{
				return HttpNotFound("Couldn't find the transport with id: " + id + " !");
			}


			//Verificare drepturi admin
			if (!User.IsInRole("Admin"))
			{
				//Utilizatorul curent
				ApplicationUser user = ctx.Users.Find(User.Identity.GetUserId());
				if (user.Id != transport.Driver.Id)
				{
					//unauthorized access
					return new HttpUnauthorizedResult();
				}
			}

			return View(transport);
		}

		// New ----------------------------
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult New()
		{
			//Construim un transport nou, fara date
			Transport transport = new Transport();
			transport.TransportDay = DateTime.Today.Day;
			transport.TransportMonth = DateTime.Today.Month;
			transport.TransportYear = DateTime.Today.Year;

			//Constuim un transport view model
			TransportViewModel transportViewModel = new TransportViewModel();
			transportViewModel.Transport = transport;
			transportViewModel.ListDrivers = GetAllDrivers();

			return View(transportViewModel);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public ActionResult New(TransportViewModel newTransport)
		{
			newTransport.ListDrivers = GetAllDrivers();
			try
			{
				newTransport.Transport.Driver = ctx.Users.Find(newTransport.DriverId);

				if (newTransport.Transport.Driver == null)
				{
					//eroare la sofer
					return View(newTransport);
				}

				//Elimina aceste campuri din verificare pentru ca ele sunt adaugate in cod
				ModelState.Remove("Transport.Driver");

				if (ModelState.IsValid)
				{
					//Adauga transportul in baza de date
					ctx.Transports.Add(newTransport.Transport);

					//Save database state
					ctx.SaveChanges();

					//In caz de succes ne duce inapoi la index
					return RedirectToAction("Index");
				}

				//Pachetul nu respecta regulile, ne intoarcem la edit
				return View(newTransport);
			}
			catch (Exception e)
			{
				//A aparut o eroare, ne intoarcem la new
				return View(newTransport);
			}
		}

        // Delete ----------------------------

        [HttpDelete]
		[Authorize(Roles = "Admin")]
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				//Nu avem parametrul id
				return HttpNotFound("Missing id parameter!");
			}

			Transport transport = ctx.Transports.Find(id);

			if (transport == null)
			{
				//transportul nu exista in baza de date
				return HttpNotFound("Couldn't find the Transport with id: " + id + " !");
			}

			ctx.Packages.Where(m => m.Transports.Contains(transport)).Load();

			ctx.Transports.Remove(transport);
			ctx.SaveChanges();

			return RedirectToAction("Index");
		}


		// Package actions -------------------------------
		[HttpGet]
		[Authorize(Roles = "Admin,Driver")]
		public ActionResult PackageDetails(int? id)
		{
			if (id == null)
			{
				//Nu avem parametrul id
				return HttpNotFound("Missing id parameter!");
			}

			//Am gasit pachetul
			return RedirectToAction("Details","Package",new { id = id});
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult AddPackage(int? id)
		{
			if (id == null)
			{
				//Nu avem parametrul id
				return HttpNotFound("Missing id parameter!");
			}
			Transport transport = ctx.Transports.Find(id);
			if (transport == null)
			{
				//transportul nu exista in baza de date
				return HttpNotFound("Couldn't find the Transport with id: " + id + " !");
			}

			TransportPackageViewModel transportPackage = new TransportPackageViewModel();
			transportPackage.TransportId = transport.TransportId;
			transportPackage.ListPackages = GetAllPackages(transport);


			return View(transportPackage);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public ActionResult AddPackage(TransportPackageViewModel newTransport)
		{
			newTransport.ListPackages = GetAllPackages(ctx.Transports.Find(newTransport.TransportId));
			try
			{
				Package package = ctx.Packages.Find(newTransport.PackageId);
				Transport transport = ctx.Transports.Find(newTransport.TransportId);

				if (package == null || transport == null)
				{
					//eroare la pachet
					return View(newTransport);
				}

                if (TryUpdateModel(transport))
                {
					transport.Packages.Add(package);

					ctx.SaveChanges();
                }

                return RedirectToAction("Details", "Transport", new { id = transport.TransportId });
			}
			catch (Exception e)
			{
				//A aparut o eroare, ne intoarcem la new
				return View(newTransport);
			}
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult RemovePackage(int? id, int? transportId)
		{
			if (id == null)
			{
				//Nu avem parametrul id
				return HttpNotFound("Missing id parameter!");
			}

			if (transportId == null)
			{
				//Nu avem parametrul id
				return HttpNotFound("Missing TransportId parameter!");
			}



			Transport transport = ctx.Transports.Find(transportId);
			Package package = ctx.Packages.Find(id);

			if (transport == null || package == null)
			{
				//transportul nu exista in baza de date
				return HttpNotFound("Couldn't find the Transport with id: " + id + " !");
			}

			if (TryUpdateModel(transport))
			{
				transport.Packages.Remove(package);

				ctx.SaveChanges();
			}

			return RedirectToAction("Details", "Transport", new { id = transport.TransportId });
		}

		// Helpers -------------------------------
		[NonAction]
		public IEnumerable<SelectListItem> GetAllPackages(Transport t)
		{
			// generam o lista goala
			var selectList = new List<SelectListItem>();
			foreach (var package in ctx.Packages.ToList())
			{
				if(!package.Transports.Contains(t))
                {
					// adaugam in lista elementele necesare pt dropdown
					selectList.Add(new SelectListItem
					{
						Value = package.PackageId.ToString(),
						Text = "Pachet " + package.PackageId.ToString()
					});
				}
				
			}
			// returnam lista pentru dropdown
			return selectList;
		}

		[NonAction]
		public IEnumerable<SelectListItem> GetAllDrivers()
		{
			// generam o lista goala
			var selectList = new List<SelectListItem>();
			foreach (var driver in ctx.Users.ToList())
			{
				var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));
				if (userManager.IsInRole(driver.Id, "Driver"))
                {
					// adaugam in lista elementele necesare pt dropdown
					selectList.Add(new SelectListItem
					{
						Value = driver.Id.ToString(),
						Text = driver.UserName
					});
				}
				
			}
			// returnam lista pentru dropdown
			return selectList;
		}
	}
}