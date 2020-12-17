using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TransportCompany.Models;

namespace TransportCompany.Controllers
{
	public class PackageController : Controller
	{
		private ApplicationDbContext ctx = new ApplicationDbContext();

		// Index -----------------------------
		[HttpGet]
		[Authorize(Roles = "Admin,Client")]
		public ActionResult Index()
		{
			List<Package> packages;
			if (User.IsInRole("Admin"))
			{
				//Afisam toate pachetele
				packages = ctx.Packages.ToList();
			}
			else
			{
				//Utilizatorul curent
				ApplicationUser user = ctx.Users.Find(User.Identity.GetUserId());
				//Daca utilizatorul curent nu e admin luam doar pachetele lui
				packages = ctx.Packages.Where(pachet => pachet.Client.Id == user.Id).ToList();
			}
			
			ViewBag.Packages = packages;

			return View();
		}

		// Details ----------------------------
		[HttpGet]
		[Authorize(Roles = "Admin,Client")]
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return HttpNotFound("Missing id parameter!");
			}
			
			//Cautam pachetul
			Package package = ctx.Packages.Find(id);

			//Verificare existenta pachet
			if (package == null)
			{
				return HttpNotFound("Couldn't find the package with id: " + id + " !");
			}


			//Verificare drepturi admin
			if (!User.IsInRole("Admin"))
			{
				//Utilizatorul curent
				ApplicationUser user = ctx.Users.Find(User.Identity.GetUserId());
				if(user.Id != package.Client.Id)
				{
					//unauthorized access
					return new HttpUnauthorizedResult();
				}
			}

			return View(package);
		}

		// New ----------------------------
		[HttpGet]
		[Authorize(Roles = "Admin,Client")]
		public ActionResult New()
		{
			//Construim un pachet nou, fara date
			Package package = new Package();

			//Constuim un package view model
			PackageViewModel packageViewModel = new PackageViewModel();
			packageViewModel.Package = package;
			packageViewModel.ListCities = GetAllCities();

			return View(packageViewModel);
		}

		[HttpPost]
		[Authorize(Roles = "Admin,Client")]
		public ActionResult New(PackageViewModel newPackage)
		{
			newPackage.ListCities = GetAllCities();
			try
			{
				//Adaugam userul conectat ca si client
				newPackage.Package.Client = ctx.Users.Find(User.Identity.GetUserId());

				newPackage.Package.ToCity = ctx.Cities.Find(newPackage.ToCityId);

				if(newPackage.Package.ToCity == null || newPackage.Package.Client == null)
				{
					//eroare la oras/client
					return View(newPackage);
				}

				//Elimina aceste campuri din verificare pentru ca ele sunt adaugate in cod
				ModelState.Remove("Package.ToCity");
				ModelState.Remove("Package.Client");

				//TODO: ModelState.isValid ??
				if (ModelState.IsValid)
				{	
					//Adauga pachetul in baza de date
					ctx.Packages.Add(newPackage.Package);

					//Save database state
					ctx.SaveChanges();

					//In caz de succes ne duce inapoi la index
					return RedirectToAction("Index");
				}

				//Pachetul nu respecta regulile, ne intoarcem la edit
				return View(newPackage);
			}
			catch (Exception e)
			{
				//A aparut o eroare, ne intoarcem la edit
				return View(newPackage);
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
			Package package = ctx.Packages.Find(id);

			if (package == null)
			{
				//Pachetul nu exista in baza de date
				return HttpNotFound("Couldn't find the package with id: " + id + " !");
			}

			//Am gasit pachetul
			return View(package);
		}

		[HttpPut]
		[Authorize(Roles = "Admin")]
		public ActionResult Edit(Package editPackage)
		{
			try
			{
				//Eliminam verificarile pe campurile pe care nu le folosim
				ModelState.Remove("ToCity");
				ModelState.Remove("Client");

				if (ModelState.IsValid)
				{
					Package package = ctx.Packages.Find(editPackage.PackageId);

					if (TryUpdateModel(package))
					{
						package.Volume = editPackage.Volume;
						package.Weight = editPackage.Weight;
						ctx.SaveChanges();
					}

					return RedirectToAction("Index");
				}

				return View(editPackage);
			}
			catch (Exception)
			{
				return View(editPackage);
			}
		}

		// Delete ----------------------------

		[HttpDelete]
		[Authorize(Roles = "Admin,Client")]
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				//Nu avem parametrul id
				return HttpNotFound("Missing id parameter!");
			}

			Package package = ctx.Packages.Find(id);
			if(!User.IsInRole("Admin"))
            {
				//Utilizatorul curent
				ApplicationUser user = ctx.Users.Find(User.Identity.GetUserId());
				if (user.Id != package.Client.Id)
				{
					//unauthorized access
					return new HttpUnauthorizedResult();
				}
			}

			if (package == null)
			{
				//Masina nu exista in baza de date
				return HttpNotFound("Couldn't find the package with id: " + id + " !");
			}

			ctx.Packages.Remove(package);
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
	}
}