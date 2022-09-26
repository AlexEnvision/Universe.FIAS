using System;
using System.Text;
using Newtonsoft.Json;
using Universe.Fias.Helpers;
using Universe.Helpers.Extensions;

namespace Universe.Fias.Normalizer.Models.AddressSystem.Field
{
    /// <summary>
    /// Address system field value House.
    /// </summary>
    public class AsAddressFieldValueHouse
    {
        private string _title;

        private string _houseIdOrCode;

        // No sort
        /// <summary>
        /// Gets or sets the ao identifier.
        /// </summary>
        /// <value>
        /// The ao identifier.
        /// </value>
        public Guid AoId { get; set; }

        /// <summary>
        /// Gets or sets the house identifier or code.
        /// </summary>
        /// <value>
        /// The house identifier.
        /// </value>
        public string HouseIdOrCode {
            get
            {
                if (_houseIdOrCode.IsNullOrEmpty() && HouseId != Guid.Empty)
                {
                    return HouseId.ToString();
                }

                return _houseIdOrCode;
            }
            set { _houseIdOrCode = value; }
        }

        /// <summary>
        /// Gets or sets the house identifier.
        /// </summary>
        /// <value>
        /// The house identifier.
        /// </value>
        [Obsolete]
        public Guid HouseId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get
            {
                if (!HouseIdOrCode.IsNullOrEmpty()
                    && HouseIdOrCode.StartsWith(AddrCodes.PrefixArbitraryAddrCode))
                {
                    return _title;
                }

                var sb = new StringBuilder();
                if (!EstStatusName.IsNullOrEmpty() && EstStatusName != "Не определено")
                {
                    sb.Append($"{EstStatusName} ");
                }

                if (!HouseNum.IsNullOrEmpty())
                {
                    sb.Append($"{HouseNum} ");
                }

                if (!BuildNum.IsNullOrEmpty())
                {
                    sb.Append($"к. {BuildNum} ");
                }

                if (!StrucNum.IsNullOrEmpty())
                {
                    if (!StrStatusName.IsNullOrEmpty() && StrStatusName != "Не определено")
                    {
                        sb.Append($"{StrStatusName} ");
                    }

                    sb.Append($"{StrucNum} ");
                }

                return sb.ToString().Trim();
            }

            set { _title = value; }
        }

        /// <summary>
        /// Gets or sets the name of the est status.
        /// </summary>
        /// <value>
        /// The name of the est status.
        /// </value>
        public string EstStatusName { get; set; }

        /// <summary>
        /// Gets or sets the house number.
        /// </summary>
        /// <value>
        /// The house number.
        /// </value>
        public string HouseNum { get; set; }

        /// <summary>
        /// Gets or sets the build number.
        /// </summary>
        /// <value>
        /// The build number.
        /// </value>
        public string BuildNum { get; set; }

        /// <summary>
        /// Gets or sets the name of the string status.
        /// </summary>
        /// <value>
        /// The name of the string status.
        /// </value>
        public string StrStatusName { get; set; }

        /// <summary>
        /// Gets or sets the struc number.
        /// </summary>
        /// <value>
        /// The struc number.
        /// </value>
        public string StrucNum { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is arbitrary.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is arbitrary; otherwise, <c>false</c>.
        /// </value>
        public bool IsArbitrary { get; set; }

        /// <summary>
        /// To the <see cref="AddrObjHouse"/>.
        /// </summary>
        /// <returns><see cref="AddrObjHouse"/>.</returns>
        public AddrObjHouse ToAddrObj()
        {
            return new AddrObjHouse
            {
                HouseId = HouseIdOrCode.ToGuidThrowIfNull(),
                EstStatusName = EstStatusName,
                HouseNum = HouseNum,
                BuildNum = BuildNum,
                StrStatusName = StrStatusName,
                StrucNum = StrucNum
            };
        }

        /// <summary>Returns a json string that represents the current object.</summary>
        /// <returns>A json string that represents the current object.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}