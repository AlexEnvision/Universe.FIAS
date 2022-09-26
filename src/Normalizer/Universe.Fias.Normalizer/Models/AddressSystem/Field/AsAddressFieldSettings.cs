using System.Collections.Generic;
using Newtonsoft.Json;
using Universe.Helpers.Extensions;

namespace Universe.Fias.Normalizer.Models.AddressSystem.Field
{
    /// <summary>
    /// Address system field settings.
    /// </summary>
    public class AsAddressFieldSettings
    {
        /// <summary>
        /// Gets or sets the address book allow edit flag group names.
        /// </summary>
        /// <value>
        /// The address book allow edit flag group names.
        /// </value>
        public List<string> AddressBookAllowEditFlagGroupNames { get; set; }

        /// <summary>
        /// Gets or sets the address book allow edit flag title.
        /// </summary>
        /// <value>
        /// The address book allow edit flag title.
        /// </value>
        public string AddressBookAllowEditFlagTitle { get; set; }

        /// <summary>
        /// Gets or sets the address book filter cond.
        /// </summary>
        /// <value>
        /// The address book filter cond.
        /// </value>
        public AddressBookFilterConditions AddressBookFilterCond { get; set; }

        /// <summary>
        /// Gets or sets the address book filter field.
        /// </summary>
        /// <value>
        /// The address book filter field.
        /// </value>
        public string AddressBookFilterField { get; set; }

        /// <summary>
        /// Gets or sets the address book flag title.
        /// </summary>
        /// <value>
        /// The address book flag title.
        /// </value>
        public string AddressBookFlagTitle { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is address book.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is address book; otherwise, <c>false</c>.
        /// </value>
        public bool IsAddressBook => !AddressBookLookupField.IsNullOrEmpty();

        /// <summary>
        /// Gets or sets the address book lookup field.
        /// </summary>
        /// <value>
        /// The address book lookup field.
        /// </value>
        public string AddressBookLookupField { get; set; }

        /// <summary>
        /// Gets or sets the address book lookup list addr field.
        /// </summary>
        /// <value>
        /// The address book lookup list addr field.
        /// </value>
        public string AddressBookLookupListAddrField { get; set; }

        /// <summary>
        /// Gets or sets the ignore address book field.
        /// </summary>
        /// <value>
        /// The ignore address book field.
        /// </value>
        public string IgnoreAddressBookField { get; set; }

        /// <summary>
        /// Gets or sets the related fields.
        /// </summary>
        /// <value>
        /// The related fields.
        /// </value>
        public List<AsAddressRelatedField> RelatedFields { get; set; } = new List<AsAddressRelatedField>();

        /// <summary>
        /// Gets or sets a value indicating whether [show street and house].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show street and house]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowStreetAndHouse { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether [requered street and house].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [requered street and house]; otherwise, <c>false</c>.
        /// </value>
        public bool RequeredStreetAndHouse { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether [arbitrary street].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [arbitrary street]; otherwise, <c>false</c>.
        /// </value>
        public bool ArbitraryStreet { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether [arbitrary house].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [arbitrary house]; otherwise, <c>false</c>.
        /// </value>
        public bool ArbitraryHouse { get; set; } = false;

        /// <summary>
        /// Gets or sets the arbitrary allow edit edit flag group names.
        /// </summary>
        /// <value>
        /// The arbitrary allow edit edit flag group names.
        /// </value>
        public List<string> ArbitraryAllowEditEditFlagGroupNames { get; set; }

        /// <summary>
        /// Parses the specified json string.
        /// </summary>
        /// <param name="str">The json string.</param>
        /// <returns></returns>
        public static AsAddressFieldSettings Parse(string str)
        {
            if (str.IsNullOrEmpty())
                return null;

            return JsonConvert.DeserializeObject<AsAddressFieldSettings>(str);
        }

        /// <summary>Returns a json string that represents the current object.</summary>
        /// <returns>A json string that represents the current object.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}