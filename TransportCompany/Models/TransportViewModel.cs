using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public string DriverId { get; set; }

        //Lista cu soferii care pot fi alesi
        public IEnumerable<SelectListItem> ListDrivers { get; set; }
    }

    public class TransportPackageViewModel
    {
        //Transportul pentru care facem modificarea
        [Required]
        public int TransportId { get; set; }

        //Id pentru pachet
        [Required]
        public int PackageId { get; set; }

        //Lista cu soferii care pot fi alesi
        public IEnumerable<SelectListItem> ListPackages { get; set; }
    }
}