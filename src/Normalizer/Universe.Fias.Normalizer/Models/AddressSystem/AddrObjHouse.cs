using System;
using System.Runtime.Serialization;

namespace Universe.Fias.Normalizer.Models.AddressSystem
{
    /// <summary>
    /// House.
    /// </summary>
    [DataContract]
    public class AddrObjHouse
    {
        /// <summary>
        /// Gets or sets the build number.
        /// </summary>
        /// <value>
        /// The build number.
        /// </value>
        [DataMember]
        public string BuildNum { get; set; }

        /// <summary>
        /// Gets or sets the name of the est status.
        /// </summary>
        /// <value>
        /// The name of the est status.
        /// </value>
        [DataMember]
        public string EstStatusName { get; set; }

        /// <summary>
        /// Gets or sets the house identifier.
        /// </summary>
        /// <value>
        /// The house identifier.
        /// </value>
        [DataMember]
        public Guid HouseId { get; set; }

        /// <summary>
        /// Gets or sets the house number.
        /// </summary>
        /// <value>
        /// The house number.
        /// </value>
        [DataMember]
        public string HouseNum { get; set; }

        /// <summary>
        /// Gets or sets the name of the string status.
        /// </summary>
        /// <value>
        /// The name of the string status.
        /// </value>
        [DataMember]
        public string StrStatusName { get; set; }

        /// <summary>
        /// Gets or sets the struc number.
        /// </summary>
        /// <value>
        /// The struc number.
        /// </value>
        [DataMember]
        public string StrucNum { get; set; }
    }
}