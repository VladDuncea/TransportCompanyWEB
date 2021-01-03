using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportCompany.Models;

namespace TransportCompany.Controllers
{
	[Authorize(Roles = "Admin")]
	public class CityController : Controller
    {
		private ApplicationDbContext ctx = new ApplicationDbContext();

		// Index -----------------------------
		[HttpGet]
		
		public ActionResult Index()
		{
			List<City> cities = ctx.Cities.ToList();
			ViewBag.Cities = cities;

			return View();
		}

		// Details ----------------------------

		[HttpGet]
        public ActionResult Details(int? id)
		{
			if (id != null)
			{
				City city = ctx.Cities.Find(id);
				if (city != null)
				{
					return View(city);
				}

				return HttpNotFound("Couldn't find the city with id: " + id + " !");
			}

			return HttpNotFound("Missing id parameter!");
		}

		// New ----------------------------

		[HttpGet]
        public ActionResult New()
		{
			//Construim o masina noua, fara date
			City city = new City();
			return View(city);
		}

		[HttpPost]
		public ActionResult New(City newCity)
		{
			try
			{
				if (ModelState.IsValid)
				{
					//Adauga orasul in baza de date
					ctx.Cities.Add(newCity);

					//Save database state
					ctx.SaveChanges();

					//In caz de succes ne duce inapoi la index
					return RedirectToAction("Index");
				}

				//Masina nu respecta regulile, ne intoarcem la edit
				return View(newCity);
			}
			catch (Exception)
			{
				//A aparut o eroare, ne intoarcem la edit
				return View(newCity);
			}
		}

		// Edit ----------------------------

		[HttpGet]
		public ActionResult Edit(int? id)
		{
			if (id != null)
			{
				//Cautam orasul
				City city = ctx.Cities.Find(id);

				if (city == null)
				{
					//Orasul nu exista in baza de date
					return HttpNotFound("Couldn't find the city with regNr: " + id + " !");
				}

				//Am gasit orasul
				return View(city);
			}

			//Nu avem parametrul id
			return HttpNotFound("Missing id parameter!");
		}

		[HttpPut]
		public ActionResult Edit(City editCity)
		{
			try
			{
				if (ModelState.IsValid)
				{
					City city = ctx.Cities.Find(editCity.CityId);

					if (TryUpdateModel(city))
					{
						city.Latitude = editCity.Latitude;
						city.Longitude = editCity.Longitude;
						ctx.SaveChanges();
					}

					return RedirectToAction("Index");
				}

				return View(editCity);
			}
			catch (Exception)
			{
				return View(editCity);
			}
		}

		// Delete ----------------------------

		[HttpDelete]
		public ActionResult Delete(int? id)
		{
			if (id != null)
			{
				City city = ctx.Cities.Find(id);

				if (city == null)
				{
					//Masina nu exista in baza de date
					return HttpNotFound("Couldn't find the city with id: " + id + " !");
				}

				ctx.Cities.Remove(city);
				ctx.SaveChanges();

				return RedirectToAction("Index");
			}

			//Nu avem parametrul id
			return HttpNotFound("Missing id parameter!");
		}
	}
}