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
	public class CarController : Controller
	{
		private ApplicationDbContext ctx = new ApplicationDbContext();

		// Index -----------------------------
		[HttpGet]
		[Authorize(Roles = "Admin,Driver")]
		public ActionResult Index()
		{
			List<Car> cars = ctx.Cars.ToList();
			ViewBag.Cars = cars;

			return View();
		}

		// Details ----------------------------

		[HttpGet]
		[Authorize(Roles = "Admin,Driver")]
		public ActionResult Details(string id)
		{
			if (id != null)
			{
				Car car = ctx.Cars.Find(id);
				if (car != null)
				{
					return View(car);
				}

				return HttpNotFound("Couldn't find the car with id: " + id + " !");
			}

			return HttpNotFound("Missing id parameter!");
		}

		// New ----------------------------

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult New()
		{
			//Construim o masina noua, fara date
			Car car = new Car();

			//Constuim un car view model
			CarViewModel carViewModel = new CarViewModel();
			carViewModel.Car = car;
			carViewModel.ListDrivers = GetAllDrivers();

			return View(carViewModel);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public ActionResult New(CarViewModel newCar)
		{

			newCar.ListDrivers = GetAllDrivers();

			try
			{
				//Get driver from id
				newCar.Car.Driver = ctx.Users.Find(newCar.DriverId);

				//Verificare cod valid
				if(newCar.Car.Driver == null)
                {
					//Nu am gasit utilizatorul
					return View(newCar);
				}

				//Eliminare verificare driver
				ModelState.Remove("Car.Driver");

				if (ModelState.IsValid)
				{
					//Adauga masina in baza de date
					ctx.Cars.Add(newCar.Car);

					//Save database state
					ctx.SaveChanges();

					//In caz de succes ne duce inapoi la index
					return RedirectToAction("Index");
				}

				//Masina nu respecta regulile, ne intoarcem la edit
				return View(newCar);
			}
			catch (Exception)
			{
				//A aparut o eroare, ne intoarcem la edit
				return View(newCar);
			}
		}

		// Edit ----------------------------

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult Edit(string id)
		{
			if (id != null)
			{
				//Cautam masina
				Car car = ctx.Cars.Find(id);

				if (car == null)
				{
					//Masina nu exista in baza de date
					return HttpNotFound("Couldn't find the car with id: " + id + " !");
				}

				//Constuim un car view model
				CarViewModel carViewModel = new CarViewModel();
				carViewModel.Car = car;
				carViewModel.ListDrivers = GetAllDrivers();
				carViewModel.DriverId = car.Driver.Id;

				//Am gasit masina
				return View(carViewModel);
			}

			//Nu avem parametrul id
			return HttpNotFound("Missing id parameter!");
		}

		[HttpPut]
		[Authorize(Roles = "Admin")]
		public ActionResult Edit(CarViewModel editCar)
		{
			editCar.ListDrivers = GetAllDrivers();

			try
			{
				//Get driver from id
				editCar.Car.Driver = ctx.Users.Find(editCar.DriverId);

				//Verificare cod valid
				if (editCar.Car.Driver == null)
				{
					//Nu am gasit utilizatorul
					return View(editCar);
				}

				//Eliminare verificare driver
				ModelState.Remove("Car.Driver");

				if (ModelState.IsValid)
				{
					Car car = ctx.Cars.Find(editCar.Car.CarId);

					if (TryUpdateModel(car))
					{
						car.Model = editCar.Car.Model;
						car.Volume = editCar.Car.Volume;
						car.Driver = editCar.Car.Driver;
						ctx.SaveChanges();
					}

					return RedirectToAction("Index");
				}

				return View(editCar);
			}
			catch (Exception)
			{
				return View(editCar);
			}
		}

		// Delete ----------------------------

		[HttpDelete]
		[Authorize(Roles = "Admin")]
		public ActionResult Delete(string id)
		{
			if (id != null)
			{
				Car car = ctx.Cars.Find(id);

				if(car == null)
                {
					//Masina nu exista in baza de date
					return HttpNotFound("Couldn't find the car with id: " + id + " !");
				}

				ctx.Cars.Remove(car);
				ctx.SaveChanges();

				return RedirectToAction("Index");
			}

			//Nu avem parametrul id
			return HttpNotFound("Missing id parameter!");
		}

		// Helpers -------------------------------
		[NonAction]
		public IEnumerable<SelectListItem> GetAllDrivers()
		{
			// generam o lista goala
			var selectList = new List<SelectListItem>();
			foreach (var driver in ctx.Users.ToList())
			{

				var userStore = new UserStore<ApplicationUser>(ctx);
				var userManager = new UserManager<ApplicationUser>(userStore);

				if (userManager.IsInRole(driver.Id,"Driver"))
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