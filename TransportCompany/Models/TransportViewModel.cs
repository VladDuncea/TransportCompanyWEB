using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TransportCompany.Models
{
    public class TransportViewModel
    {
        //Utilizatorul
        public Transport Transport { get; set; }

        //Id pentru sofer
        public int DriverId { get; set; }

        //Lista cu soferii care pot fi alesi
        public IEnumerable<SelectListItem> ListDrivers { get; set; }
    }
}