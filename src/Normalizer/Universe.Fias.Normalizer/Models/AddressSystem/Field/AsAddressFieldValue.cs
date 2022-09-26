using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Universe.Helpers.Extensions;

namespace Universe.Fias.Normalizer.Models.AddressSystem.Field
{
    /// <summary>
    /// Address system field value.
    /// </summary>
    public class AsAddressFieldValue
    {
        // No sort        
        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        public AsAddressFieldValueAddrObj Region { get; set; }

        /// <summary>
        /// Gets or sets the district.
        /// </summary>
        /// <value>
        /// The district.
        /// </value>
        public AsAddressFieldValueAddrObj District { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>
        /// The city.
        /// </value>
        public AsAddressFieldValueAddrObj City { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        /// <value>
        /// The locality.
        /// </value>
        public AsAddressFieldValueAddrObj Locality { get; set; }

        /// <summary>
        /// Gets or sets the element of the planning structure.
        /// </summary>
        /// <value>
        /// The street.
        /// </value>
        public AsAddressFieldValueAddrObj ElmOfPlanStruct { get; set; }

        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        /// <value>
        /// The street.
        /// </value>
        public AsAddressFieldValueAddrObj Street { get; set; }

        /// <summary>
        /// Gets or sets the arbitrary street.
        /// </summary>
        /// <value>
        /// The arbitrary street.
        /// </value>
        public bool? ArbitraryStreet { get; set; }

        /// <summary>
        /// Gets or sets the house.
        /// </summary>
        /// <value>
        /// The house.
        /// </value>
        public AsAddressFieldValueHouse House { get; set; }

        /// <summary>
        /// Gets or sets the arbitrary house.
        /// </summary>
        /// <value>
        /// The arbitrary house.
        /// </value>
        public bool? ArbitraryHouse { get; set; }

        /// <summary>
        /// Gets or sets the address book allow edit flag.
        /// </summary>
        /// <value>
        /// The address book allow edit flag.
        /// </value>
        public bool? AddressBookAllowEditFlag { get; set; }

        /// <summary>
        /// Gets or sets the address book flag.
        /// </summary>
        /// <value>
        /// The address book flag.
        /// </value>
        public bool? AddressBookFlag { get; set; }

        /// <summary>
        /// Gets or sets the address book lookup identifier.
        /// </summary>
        /// <value>
        /// The address book lookup identifier.
        /// </value>
        public int? AddressBookLookupId { get; set; }

        /// <summary>
        /// Gets or sets the address book lookup title.
        /// </summary>
        /// <value>
        /// The address book lookup title.
        /// </value>
        public string AddressBookLookupTitle { get; set; }

        /// <summary>
        /// Gets or sets the name of the place.
        /// </summary>
        /// <value>
        /// The name of the place.
        /// </value>
        public string PlaceName { get; set; }

        /// <summary>Returns a json string that represents the current object.</summary>
        /// <returns>A json string that represents the current object.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Gets the full address by template:
        /// [Region] [District] [City] [Locality] [Street] [House].
        /// </summary>
        public string GetFullAddress()
        {
            var sb = new StringBuilder();
            if (Region != null)
                sb.Append(Region.Title);

            if (District != null)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(District.Title);
            }

            if (City != null && Region?.Code != City.Code)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(City.Title);
            }

            if (Locality != null)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(Locality.Title);
            }

            if (!PlaceName.IsNullOrEmpty())
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(PlaceName);
            }

            if (ElmOfPlanStruct != null)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(ElmOfPlanStruct.Title);
            }

            if (Street != null)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(Street.Title);
            }

            if (House != null)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(House.Title);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the place by template:
        /// [City] [Locality] [Place].
        /// </summary>
        public string GetPlace()
        {
            var sb = new StringBuilder();
            if (City != null)
                sb.Append(City.Title);

            if (Locality != null)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(Locality.Title);
            }

            if (!PlaceName.IsNullOrEmpty())
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(PlaceName);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns true if this value is valid.
        /// </summary>
        /// <param name="required">if set to <c>true</c> [required].</param>
        /// <param name="requeredPlaceName">if set to <c>true</c> [requered place name].</param>
        /// <param name="requeredStreetAndHouse">if set to <c>true</c> [requered street and house].</param>
        /// <returns></returns>
        public string IsValid(bool required, bool requeredPlaceName, bool requeredStreetAndHouse)
        {
            var invalisFields = new List<string>();

            if (Region == null)
                invalisFields.Add("Region");


            if (District == null && City == null && Locality == null && Street == null && House == null)
                invalisFields.Add("District and/or City and/or Locality");
            else if (requeredStreetAndHouse && City == null && Locality == null && Street == null && House == null)
                invalisFields.Add("City or Locality");

            if (requeredPlaceName && PlaceName.IsNullOrEmpty() && (Street != null || House != null))
                invalisFields.Add("Place name");

            // 5.1.1 п. 57-58 –  поля Улица и Дом сделаны опциональными. На этапе планирования Инициатор знает только город, поэтому поля улица и дом необязательные.
            if (requeredStreetAndHouse && Street == null && House == null)
                invalisFields.Add("Street");

            if (requeredStreetAndHouse && House == null)
                invalisFields.Add("House");

            if (required && invalisFields.Count > 0)
                return $"You must specify a value for this required address fields: {invalisFields.Aggregate((s1, s2) => s1 + ", " + s2)}.";

            return string.Empty;
        }
    }
}