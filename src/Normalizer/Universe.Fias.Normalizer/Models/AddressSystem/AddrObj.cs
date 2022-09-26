using System;
using System.Runtime.Serialization;

namespace Universe.Fias.Normalizer.Models.AddressSystem
{
    /// <summary>
    /// Address object.
    /// </summary>
    [DataContract]
    public class AddrObj
    {
        /// <summary>
        /// Gets or sets the ao identifier.
        /// </summary>
        /// <value>
        /// The ao identifier.
        /// </value>
        [DataMember]
        public Guid AoId { get; set; }

        /// <summary>
        /// Gets or sets the ao parent identifier.
        /// </summary>
        /// <value>
        /// The ao parent identifier.
        /// </value>
        [DataMember]
        public Guid? ParentAoId { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the district.
        /// </summary>
        /// <value>
        /// The district.
        /// </value>
        [DataMember]
        public AddrObj District { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        [DataMember]
        public AddrObj Region { get; set; }

        /// <summary>
        /// Gets or sets the short name of the type.
        /// </summary>
        /// <value>
        /// The short name of the type.
        /// </value>
        [DataMember]
        public string TypeShortName { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        [DataMember]
        public AddrObj Parent { get; set; }
    }
}