using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransportCompany.Models
{
	public class Package
	{
		[Key]
		public string PackageId { get; set; }

		[Required,
			Range(0, 150, ErrorMessage = "Value for {0} must be between {1} and {2}!")]
		public float Volume { get; set; }

		[Required,
			Range(0, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}!")]
		public float Weight { get; set; }

		//Foreign keys with City
		//[Required]
		//public virtual City FromCity { get; set; }

		[Required]
		public virtual City ToCity { get; set; }

		[Required]
		public virtual ApplicationUser Client { get; set; }
	}
}