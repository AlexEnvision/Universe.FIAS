using System.Runtime.Serialization;

namespace Universe.Fias.Normalizer.Models.AddressSystem
{
    /// <summary>
    /// Address from book.
    /// </summary>
    [DataContract]
    public class AddressFromBook
    {
        // No Sort

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        /// <value>
        /// The street.
        /// </value>
        [DataMember]
        public AddrObj Street { get; set; }

        /// <summary>
        /// Gets or sets the house.
        /// </summary>
        /// <value>
        /// The house.
        /// </value>
        [DataMember]
        public AddrObjHouse House { get; set; }
    }
}