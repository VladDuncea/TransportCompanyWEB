using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransportCompany.Models
{
    public class Car
    {
        [Key]
        [MinLength(5, ErrorMessage = "Registration number cannot be less than {0} characters"),
            MaxLength(10, ErrorMessage = "Registration number cannot be more than {1} characters!")]
        public string RegistrationNr { get; set; }

        [MinLength(5, ErrorMessage = "Model cannot be less than {0} characters!"),
            MaxLength(100, ErrorMessage = "Registration cannot be more than {1} characters!")]
        public string Model { get; set; }

        [Required]
        public float Volume { get; set; }
    }
}