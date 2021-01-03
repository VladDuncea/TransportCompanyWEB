using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TransportCompany.Models
{
    public class PackageViewModel
    {
        //Utilizatorul
        public Package Package { get; set; }
        
        //Id pentru orasul de destinatie
        [Required(ErrorMessage = "City is required")]
        public int ToCityId { get; set; }

        //Lista cu orasele care pot fi alese
        public IEnumerable<SelectListItem> ListCities { get; set; }
    }
}