using Universe.CQRS.Models.Base;

namespace Universe.Fias.Normalizer.Algorithms.AddressSystem.Utils
{
	public class AddressTypeDto : EntityDto
	{
        public int Level { get; set; }

		public string ShortName { get; set; }

		public string Name { get; set; }
	}
}
