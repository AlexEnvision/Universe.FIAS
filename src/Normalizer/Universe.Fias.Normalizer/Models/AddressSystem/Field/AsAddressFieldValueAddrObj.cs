using System;
using Newtonsoft.Json;

namespace Universe.Fias.Normalizer.Models.AddressSystem.Field
{
    /// <summary>
    /// Address system field value AddrObj.
    /// </summary>
    public class AsAddressFieldValueAddrObj
    {
        // No sort
        /// <summary>
        /// Gets or sets the ao identifier.
        /// </summary>
        /// <value>
        /// The ao identifier.
        /// </value>
        public Guid AoId { get; set; }

        /// <summary>
        /// Gets or sets the parent ao identifier.
        /// </summary>
        /// <value>
        /// The ao identifier.
        /// </value>
        public Guid? ParentAoId { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the short name of the type.
        /// </summary>
        /// <value>
        /// The short name of the type.
        /// </value>
        public string TypeShortName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is arbitrary.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is arbitrary; otherwise, <c>false</c>.
        /// </value>
        public bool IsArbitrary { get; set; }

        /// <summary>Returns a json string that represents the current object.</summary>
        /// <returns>A json string that represents the current object.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// To the <see cref="AddrObj"/>.
        /// </summary>
        /// <returns><see cref="AddrObj"/>.</returns>
        public AddrObj ToAddrObj()
        {
            return new AddrObj {
                AoId = AoId,
                Code = Code,
                TypeShortName = TypeShortName,
                Name = Name
            };
        }
    }
}