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
		public ActionResult Details(string regNr)
		{
			if(regNr != null)
			{
				Car car = ctx.Cars.Find(regNr);
				if(car != null)
				{
					return View(car);
				}
				
				return HttpNotFound("Couldn't find the car with regNr: " + regNr + " !");
			}

			return HttpNotFound("Missing book id parameter!");
		}
	}
}