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
			PackageVolumeValidator]
		public int Volume { get; set; }

		[Required,
			Range(1, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}!")]
		public float Weight { get; set; }

		[Required]
		//One-to-many
		public virtual City ToCity { get; set; }

		[Required]
		//One-to-many
		public virtual ApplicationUser Client { get; set; }

		//Many-to-many relationship
		public virtual ICollection<Transport> Transports { get; set; }
	}
}