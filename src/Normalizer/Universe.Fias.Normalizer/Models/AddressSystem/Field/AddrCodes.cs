using Newtonsoft.Json;
using Universe.Helpers.Extensions;

namespace Universe.Fias.Normalizer.Models.AddressSystem.Field
{
    /// <summary>
    /// Address codes.
    /// </summary>
    public class AddrCodes
    {
        /// <summary>
        /// Gets or sets the prefix arbitrary addr code.
        /// </summary>
        /// <value>
        /// The region code.
        /// </value>
        public static string PrefixArbitraryAddrCode => "arbitrary:|";

        /// <summary>
        /// Gets or sets the region code.
        /// </summary>
        /// <value>
        /// The region code.
        /// </value>
        public string RegionCode { get; set; }

        /// <summary>
        /// Gets or sets the district code.
        /// </summary>
        /// <value>
        /// The district code.
        /// </value>
        public string DistrictCode { get; set; }

        /// <summary>
        /// Gets or sets the city code.
        /// </summary>
        /// <value>
        /// The city code.
        /// </value>
        public string CityCode { get; set; }

        /// <summary>
        /// Gets or sets the locality code.
        /// </summary>
        /// <value>
        /// The locality code.
        /// </value>
        public string LocalityCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the place.
        /// </summary>
        /// <value>
        /// The name of the place.
        /// </value>
        public string PlaceName { get; set; }

        /// <summary>
        /// Gets or sets the street code.
        /// </summary>
        /// <value>
        /// The street code.
        /// </value>
        public string StreetCode { get; set; }

        /// <summary>
        /// Gets or sets the arbitrary street.
        /// </summary>
        /// <value>
        /// The arbitrary street.
        /// </value>
        public bool? ArbitraryStreet { get; set; }

        /// <summary>
        /// Gets or sets the house code.
        /// </summary>
        /// <value>
        /// The house code.
        /// </value>
        public string HouseCode { get; set; }

        /// <summary>
        /// Gets or sets the arbitrary house.
        /// </summary>
        /// <value>
        /// The arbitrary house.
        /// </value>
        public bool? ArbitraryHouse { get; set; }

        /// <summary>
        /// Parses the specified json string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static AddrCodes Parse(string str)
        {
            if (str.IsNullOrEmpty())
                return null;

            return JsonConvert.DeserializeObject<AddrCodes>(str);
        }

        /// <summary>
        ///     Проверка, является ли этот объект пустым
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return RegionCode.IsNullOrEmpty() && 
                   DistrictCode.IsNullOrEmpty() && 
                   CityCode.IsNullOrEmpty() && 
                   LocalityCode.IsNullOrEmpty() && 
                   StreetCode.IsNullOrEmpty() && 
                   HouseCode.IsNullOrEmpty();
        }

        /// <summary>Returns a json string that represents the current object.</summary>
        /// <returns>A json string that represents the current object.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}