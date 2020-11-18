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

				return HttpNotFound("Couldn't find the car with regNr: " + id + " !");
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
			return View(car);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public ActionResult New(Car newCar)
		{
			try
			{
				if (ModelState.IsValid)
				{
					//Adauga masina in baza de date
					ctx.Cars.Add(newCar);

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
					return HttpNotFound("Couldn't find the car with regNr: " + id + " !");
				}

				//Am gasit masina
				return View(car);
			}

			//Nu avem parametrul id
			return HttpNotFound("Missing id parameter!");
		}

		[HttpPut]
		[Authorize(Roles = "Admin")]
		public ActionResult Edit(Car editCar)
		{
			try
			{
				if (ModelState.IsValid)
				{
					Car car = ctx.Cars.Find(editCar.RegistrationNr);

					if (TryUpdateModel(car))
					{
						car.Model = editCar.Model;
						car.Volume = editCar.Volume;
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
					return HttpNotFound("Couldn't find the car with regNr: " + id + " !");
				}

				ctx.Cars.Remove(car);
				ctx.SaveChanges();

				return RedirectToAction("Index");
			}

			//Nu avem parametrul id
			return HttpNotFound("Missing id parameter!");
		}

	}
}