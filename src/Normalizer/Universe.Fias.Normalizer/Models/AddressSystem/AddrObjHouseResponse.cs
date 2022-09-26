using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Universe.Fias.Normalizer.Models.AddressSystem
{
    /// <summary>
    /// Response houses.
    /// </summary>
    /// <seealso cref="IErrorResponse" />
    [DataContract]
    public class AddrObjHouseResponse : IErrorResponse
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [DataMember]
        public List<AddrObjHouse> Items { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        [DataMember]
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        [DataMember]
        public ErrorResponse Error { get; set; }
    }
}