using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Universe.Fias.Normalizer.Models.AddressSystem
{
    /// <summary>
    /// Response address objects.
    /// </summary>
    /// <seealso cref="IErrorResponse" />
    [DataContract]
    public class AddrObjResponse : IErrorResponse
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [DataMember]
        public List<AddrObj> Items { get; set; }

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