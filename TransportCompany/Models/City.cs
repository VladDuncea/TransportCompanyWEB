using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransportCompany.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Required,
            MinLength(3, ErrorMessage = "Value for {0} must be at least {1} characters long!"),
            MaxLength(100, ErrorMessage = "Value for {0} must be at most {1} characters long!")]
        public string Name { get; set; }

        [Required,
            Range(-180.0,180.0,ErrorMessage = "Value for {0} must be between {1} and {2}!")]
        public float Longitude { get; set; }

        [Required,
            Range(-90.0, 90.0, ErrorMessage = "Value for {0} must be between {1} and {2}!")]
        public float Latitude { get; set; }

        // many-to-one relationship
        public virtual ICollection<Package> Packages { get; set; }
    }
}