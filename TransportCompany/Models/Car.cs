using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TransportCompany.Models
{
    public class Car
    {
        public string CarId { get; set; }

        //[Index("RegNr_Unique", IsUnique = true)]
        [Required, RegularExpression(@"^(([A-Z]){1}|([A-Z]){2})-((\d{2})|(\d{3}))-([A-Z]){3}$", ErrorMessage = "This is not a car reg. number!")]
        public string RegistrationNr { get; set; }

        [Required,MinLength(5, ErrorMessage = "{0} cannot be less than {1} characters!"),
            MaxLength(100, ErrorMessage = "{0} cannot be more than {1} characters!")]
        public string Model { get; set; }

        [Required,
            Range(1,1000,ErrorMessage = "Value for {0} must be between {1} and {2}!")]
        public float Volume { get; set; }

        //One-to-One cu soferul
        [Required]
        public virtual ApplicationUser Driver { get; set; }
    }
}