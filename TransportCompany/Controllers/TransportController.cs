using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
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
				//Afisam toate pachetele
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
			//Construim un pachet nou, fara date
			Transport transport = new Transport();

			//Constuim un package view model
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
					//Adauga pachetul in baza de date
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
				//A aparut o eroare, ne intoarcem la edit
				return View(newTransport);
			}
		}

		// Edit ----------------------------

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				//Nu avem parametrul id
				return HttpNotFound("Missing id parameter!");
			}
			//Cautam pachetul
			Transport transport = ctx.Transports.Find(id);

			if (transport == null)
			{
				//Pachetul nu exista in baza de date
				return HttpNotFound("Couldn't find the Transport with id: " + id + " !");
			}

			//Am gasit pachetul
			return View(transport);
		}

		//[HttpPut]
		//[Authorize(Roles = "Admin")]
		//public ActionResult Edit(Transport editTransport)
		//{
		//	try
		//	{
		//		//Eliminam verificarile pe campurile pe care nu le folosim
		//		ModelState.Remove("ToCity");
		//		ModelState.Remove("Client");

		//		if (ModelState.IsValid)
		//		{
		//			Package package = ctx.Packages.Find(editTransport.PackageId);

		//			if (TryUpdateModel(package))
		//			{
		//				package.Volume = editTransport.Volume;
		//				package.Weight = editTransport.Weight;
		//				ctx.SaveChanges();
		//			}

		//			return RedirectToAction("Index");
		//		}

		//		return View(editTransport);
		//	}
		//	catch (Exception)
		//	{
		//		return View(editTransport);
		//	}
		//}

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
				//Masina nu exista in baza de date
				return HttpNotFound("Couldn't find the Transport with id: " + id + " !");
			}

			ctx.Transports.Remove(transport);
			ctx.SaveChanges();

			return RedirectToAction("Index");
		}


		// Helpers -------------------------------
		[NonAction]
		public IEnumerable<SelectListItem> GetAllCities()
		{
			// generam o lista goala
			var selectList = new List<SelectListItem>();
			foreach (var city in ctx.Cities.ToList())
			{
				// adaugam in lista elementele necesare pt dropdown
				selectList.Add(new SelectListItem
				{
					Value = city.CityId.ToString(),
					Text = city.Name
				});
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