using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransportCompany.Models
{
    public class Transport
    {
        [Key]
        public int TransportId { get; set; }

        [Required,
            MinLength(3, ErrorMessage = "Value for {0} must be at least {1} characters long!"),
            MaxLength(100, ErrorMessage = "Value for {0} must be at most {1} characters long!")]
        public string Name { get; set; }

        [Required, 
            RegularExpression(@"^(([1-9])|([12]\d)|(3[01]))$", ErrorMessage = "This is not a valid day!"),
            TransportDayValidator]
        public int TransportDay { get; set; }

        [Required, 
            RegularExpression(@"^(0[1-9])|(1[012])|([1-9])$", ErrorMessage = "This is not a valid month!")]
        public int TransportMonth { get; set; }

        [Required, 
            RegularExpression(@"^[1-2](\d{3})$", ErrorMessage = "This is not a valid year!")]
        public int TransportYear { get; set; }

        // many-to-one relationship
        public virtual ICollection<Package> Packages { get; set; }

        // one-to-many relationship
        [Required]
        public virtual ApplicationUser Driver { get; set; }
    }
}