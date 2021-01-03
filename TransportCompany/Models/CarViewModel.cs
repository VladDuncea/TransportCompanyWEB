using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TransportCompany.Models
{
    public class CarViewModel
    {
        //Masina
        public Car Car { get; set; }

        //Id pentru sofer de destinatie
        [Required(ErrorMessage ="Please select a driver!")]
        public string DriverId { get; set; }

        //Lista cu orasele care pot fi alese
        public IEnumerable<SelectListItem> ListDrivers { get; set; }
    }
}