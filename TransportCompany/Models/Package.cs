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
		public int PackageId { get; set; }

		[Required,
			Range(1, 150, ErrorMessage = "Value for {0} must be between {1} and {2}!")]
		public float Volume { get; set; }

		[Required,
			Range(1, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}!")]
		public float Weight { get; set; }

		//Foreign keys with City
		//TODO: ask how to have two keys to city
		//[Required]
		//public virtual City FromCity { get; set; }

		[Required]
		//One-to-many
		public virtual City ToCity { get; set; }

		[Required]
		//One-to-many
		public virtual ApplicationUser Client { get; set; }

		//One-to-many relationship
		public virtual Transport Transport { get; set; }
	}
}