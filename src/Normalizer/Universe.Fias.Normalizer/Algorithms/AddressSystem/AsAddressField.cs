using Newtonsoft.Json;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.Normalizer.Algorithms.AddressSystem.Base;
using Universe.Fias.Normalizer.Algorithms.AddressSystem.Settings;
using Universe.Fias.Normalizer.Models.AddressSystem.Field;
using Universe.Helpers.Extensions;
using Universe.Types;

namespace Universe.Fias.Normalizer.Algorithms.AddressSystem
{
    public class AsAddressField : DisposableObject, IAsAddressField
    {
        private readonly UniverseFiasScope _scope;

        public AsAddressFieldSettings Settings { get; set; }

        public AsAddressField(UniverseFiasScope scope)
        {
            _scope = scope;
        }

        /// <summary>
        ///     Построение значения.
        ///     Builds the value.
        /// </summary>
        /// <param name="codes">
        ///     Коды адресов.
        ///     The codes.
        /// </param>
        /// <param name="settings">
        ///     Настройки построения адреса.
        ///     Build address settings.
        /// </param>
        /// <returns></returns>
        public AsAddressFieldValue BuildValue(AddrCodes codes, BuildAddressSettings settings = null)
        {
            if (codes == null)
                return null;

            if (settings == null)
                settings = BuildAddressSettings.Default;

            if (codes.RegionCode.IsNullOrEmpty() && codes.DistrictCode.IsNullOrEmpty()
                                                 && codes.CityCode.IsNullOrEmpty() && codes.LocalityCode.IsNullOrEmpty()
                                                 && codes.StreetCode.IsNullOrEmpty() && codes.HouseCode.IsNullOrEmpty())
                return null;

            var value = new AsAddressFieldValue();

            using (var oper = new AsAddrObjsBOper(_scope))
            {
                value.Region = GetValueAddrObj(oper, codes.RegionCode);
                value.District = GetValueAddrObj(oper, codes.DistrictCode);
                value.City = GetValueAddrObj(oper, codes.CityCode);
                value.Locality = GetValueAddrObj(oper, codes.LocalityCode);

                value.PlaceName = codes.PlaceName;

                value.Street = GetValueAddrObj(oper, codes.StreetCode);
                value.ArbitraryStreet = codes.ArbitraryStreet;

                var parent = value.Locality ?? value.City ?? value.District ?? value.Region;
                if (value.Street != null && value.Street.ParentAoId != null && parent != null && value.Street.ParentAoId != parent.AoId)
                    value.ElmOfPlanStruct = oper.GetAddrObjByAoIdSafe(value.Street.ParentAoId.Value);

                string parentCodeForHouse;
                if (!codes.StreetCode.IsNullOrEmpty())
                    parentCodeForHouse = codes.StreetCode;
                else if (!codes.LocalityCode.IsNullOrEmpty())
                    parentCodeForHouse = codes.LocalityCode;
                else if (!codes.CityCode.IsNullOrEmpty())
                    parentCodeForHouse = codes.CityCode;
                else if (!codes.DistrictCode.IsNullOrEmpty())
                    parentCodeForHouse = codes.DistrictCode;
                else
                    parentCodeForHouse = codes.RegionCode;

                switch (settings.HousesMatchMode)
                {
                    case HousesMatchMode.AoParentExactSingle:
                    case HousesMatchMode.AoParentExactMultiple:
                        value.House = GetValueHouse(oper, parentCodeForHouse, codes.HouseCode);
                        break;

                    case HousesMatchMode.OnlyHouseNumbers:
                        value.House = GetValueHouseByOnlyHouseId(oper, parentCodeForHouse, codes.HouseCode);
                        break;
                }
                
                value.ArbitraryHouse = codes.ArbitraryHouse;
            }

            return value;
        }

        /// <summary>
        /// Gets the value addr object.
        /// </summary>
        /// <param name="oper">The oper.</param>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        private AsAddressFieldValueAddrObj GetValueAddrObj(AsAddrObjsBOper oper, string code)
        {
            if (code.IsNullOrEmpty())
                return null;

            return oper.GetAddrObj(code);
        }

        /// <summary>
        /// Gets the value house.
        /// </summary>
        /// <param name="oper">The oper.</param>
        /// <param name="parentCode">The parent code.</param>
        /// <param name="houseCode">The house code.</param>
        /// <returns></returns>
        private AsAddressFieldValueHouse GetValueHouse(AsAddrObjsBOper oper, string parentCode, string houseCode)
        {
            if (houseCode.IsNullOrEmpty())
                return null;

            return oper.GetHouse(parentCode, houseCode);
        }

        /// <summary>
        /// Gets the value house.
        /// </summary>
        /// <param name="oper">The oper.</param>
        /// <param name="parentCode">The parent code.</param>
        /// <param name="houseCode">The house code.</param>
        /// <returns></returns>
        private AsAddressFieldValueHouse GetValueHouseByOnlyHouseId(AsAddrObjsBOper oper, string parentCode, string houseCode)
        {
            if (houseCode.IsNullOrEmpty())
                return null;

            return oper.GetHouseByOnlyHouseId(parentCode, houseCode);
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    _db?.Dispose();
            //    _db = null;
            //}
        }

        public string GetFieldValueObjAsText(AsAddressFieldValue value)
        {
            if (value == null)
                return null;

            return JsonConvert.SerializeObject(value);
        }
    }
}