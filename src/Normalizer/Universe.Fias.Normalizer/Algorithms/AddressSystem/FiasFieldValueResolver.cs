using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.DataAccess.Models;
using Universe.Fias.Normalizer.Algorithms.AddressSystem.Settings;
using Universe.Fias.Normalizer.Algorithms.AddressSystem.Utils;
using Universe.Fias.Normalizer.Models.AddressSystem;
using Universe.Fias.Normalizer.Models.AddressSystem.Field;
using Universe.Helpers.Extensions;
using Universe.Types.Collection;

namespace Universe.Fias.Normalizer.Algorithms.AddressSystem
{
    /// <summary>
    ///     Класс-разрешитель поля формата ФИАС
    ///     FIAS field value resolver class
    /// </summary>
    public class FiasFieldValueResolver
    {
        private readonly UniverseFiasScope _scope;
        private readonly IUniverseLogger _log;
        private readonly IFiasUtil _fiasUtil;

        /// <summary>
        ///     Утилита, работающая с фрагментами адреса
        /// </summary>
        public IFiasUtil FiasUtil => _fiasUtil;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FiasFieldValueResolver" /> class.
        /// </summary>
        public FiasFieldValueResolver(UniverseFiasScope scope, IUniverseLogger log)
        {
            _scope = scope;
            _log = log;

            _fiasUtil = new FiasUtil(scope);
        }

        /// <summary>
        ///     Нормализация адреса.
        /// </summary>
        /// <param name="address">Адрес для нормализации</param>
        /// <param name="settings">Настройки нормализации. Null - значение настроек по-умолчанию.</param>
        /// <returns>
        ///     Возвращает:
        ///     Коды адресов так называемый адрес в нормализованном виде.
        /// </returns>
        public AddrCodes NormalizeAddress(string address, NormalizeAddressSettings settings = null)
        {
            if (settings == null)
                settings = NormalizeAddressSettings.Default;

            if (address.IsNullOrEmpty())
                return new AddrCodes();

            using (var oper = new AsAddrObjsBOper(_scope))
            {
                var addrCodes = NormalizeAddressInternal(address, oper, settings);
                return addrCodes;
            }
        }

        private AddrCodes NormalizeAddressInternal(string address, AsAddrObjsBOper oper, NormalizeAddressSettings settings = null)
        {
            if (settings == null)
                settings = NormalizeAddressSettings.Default;

            var addrCodes = new AddrCodes();
            if (address.IsNullOrEmpty())
                return addrCodes;

            address = AddressPreprocessing(address);
            var candidates = Regex.Split(address, @"[!@#$%|,_();]").Where(x => !x.IsNullOrWhiteSpace()).ToArray();

            var regionCode = TryGetRegionCodeMultiple(oper, settings.RegionExtendSearchPatterns, candidates);
            if (!regionCode.IsNullOrEmpty())
            {
                addrCodes.RegionCode = regionCode;
            }

            var districtCode = TryGetDistrictCodeMiltiple(oper, ref regionCode, settings.DistrictExtendSearchPatterns, candidates);
            if (!districtCode.IsNullOrEmpty())
            {
                addrCodes.DistrictCode = districtCode;
            }

            var cityCode = TryGetCityCodeMultiple(oper, ref regionCode, ref districtCode, settings.SettlementExtendSearchPatterns, candidates);
            if (!cityCode.IsNullOrEmpty())
            {
                addrCodes.CityCode = cityCode;
            }

            var localityCode = TryGetLocalityCodeMultiple(oper, ref regionCode, ref districtCode, cityCode, candidates);
            if (!localityCode.IsNullOrEmpty())
            {
                addrCodes.LocalityCode = localityCode;
            }

            switch (settings.HousesMatchMode)
            {
                case HousesMatchMode.AoParentExactSingle:
                    var streetCode = TryGetStreetCodes(oper, settings, cityCode, localityCode, candidates).FirstOrDefault();
                    if (!streetCode.IsNullOrEmpty())
                    {
                        addrCodes.StreetCode = streetCode;
                    }

                    if (settings.IncludeHouses)
                    {
                        var houseId = TryGetHouseIdMultiple(oper, streetCode, candidates);
                        if (houseId != null && !streetCode.IsNullOrEmpty())
                        {
                            addrCodes.HouseCode = houseId.ToString();
                        }
                    }

                    break;

                case HousesMatchMode.AoParentExactMultiple:
                    var streetCodes = TryGetStreetCodes(oper, settings, cityCode, localityCode, candidates);
                    if (streetCodes.Length == 0)
                    {
                        // Берём первый разрешённый код улицы
                        // TODO -> В методе получения домов сделать подмену кода улицы, если найден дом
                        addrCodes.StreetCode = streetCodes.FirstOrDefault();
                    }

                    if (settings.IncludeHouses)
                    {
                        // TODO -> Сделать подмену кода улицы, если найден дом
                        var houseId = TryGetHouseIdMultiple(oper, streetCodes, candidates);
                        if (houseId != null && streetCodes.Length > 0)
                        {
                            addrCodes.HouseCode = houseId.ToString();
                        }
                    }

                    break;

                case HousesMatchMode.OnlyHouseNumbers:
                    var streetCodeSingle = TryGetStreetCodes(oper, settings, cityCode, localityCode, candidates).FirstOrDefault();
                    if (!streetCodeSingle.IsNullOrEmpty())
                    {
                        addrCodes.StreetCode = streetCodeSingle;
                    }

                    if (settings.IncludeHouses)
                    {
                        var houseId = TryGetHouseIdByOnlyHouseNumberMultiple(oper, candidates);
                        if (houseId != null && !streetCodeSingle.IsNullOrEmpty())
                        {
                            addrCodes.HouseCode = houseId.ToString();
                        }
                    }

                    break;
            }

            return addrCodes;
        }

        private string[] TryGetStreetCodes(AsAddrObjsBOper oper, NormalizeAddressSettings settings, string cityCode, string localityCode,
            params string[] candidates)
        {
            // Признак игнорирования проверки на актуальность у улицы.
            var actualIgnore = settings.ActualIgnore;

            switch (settings.StreetMatchMode)
            {
                case StreetMatchMode.AoParentExact:
                    if (settings.HousesMatchMode == HousesMatchMode.AoParentExactMultiple)
                    {
                        var streetCodes = TryGetStreetCodesMultiple(oper, cityCode, localityCode, actualIgnore, candidates);
                        return streetCodes;
                    }

                    var streetCode = TryGetStreetCodeMultiple(oper, cityCode, localityCode, actualIgnore, candidates);
                    return new[] { streetCode};

                case StreetMatchMode.OnlyStreetNames:
                    var streetCodeByNames = TryGetStreetByOnlyStreetNamesCodeMultiple(oper, candidates);
                    return new[] { streetCodeByNames };

                default:
                    var streetCodeDefault = TryGetStreetCodeMultiple(oper, cityCode, localityCode, actualIgnore, candidates);
                    return new[] { streetCodeDefault };
            }
        }

        /// <summary>
        ///     Нормализация города, либо населённого пункта.
        /// </summary>
        /// <param name="settlement">Город, либо населённый пункт для нормализации</param>
        /// <returns>
        ///     Возвращает:
        ///     Коды города (CityCode), либо населённый пункт (LocalityCode) так называемый адрес в нормализованном виде.
        /// </returns>
        public AddrCodes NormalizeSettlement(string settlement)
        {
            if (settlement.IsNullOrEmpty())
                return new AddrCodes();

            using (var oper = new AsAddrObjsBOper(_scope))
            {
                var addrCodes = NormalizeSettlementInternal(settlement, oper);
                return addrCodes;
            }
        }

        private AddrCodes NormalizeSettlementInternal(string settlement, AsAddrObjsBOper oper)
        {
            var addrCodes = new AddrCodes();
            if (settlement.IsNullOrEmpty())
                return addrCodes;

            settlement = AddressPreprocessing(settlement);
            var candidates = Regex.Split(settlement, @"[!@#$%|,_();]");

            var regionCode = TryGetRegionCodeMultiple(oper, true, candidates);
            if (!regionCode.IsNullOrEmpty())
            {
                addrCodes.RegionCode = regionCode;
            }

            var districtCode = TryGetDistrictCodeMiltiple(oper, ref regionCode, true, candidates);
            if (!districtCode.IsNullOrEmpty())
            {
                addrCodes.DistrictCode = districtCode;
            }

            var cityCode = TryGetCityCodeMultiple(oper, ref regionCode, ref districtCode, true, candidates);
            if (!cityCode.IsNullOrEmpty())
            {
                addrCodes.CityCode = cityCode;
            }

            var localityCode = TryGetLocalityCodeMultiple(oper, ref regionCode, ref districtCode, cityCode, candidates);
            if (!localityCode.IsNullOrEmpty())
            {
                addrCodes.LocalityCode = localityCode;
            }

            return addrCodes;
        }

        private static string AddressPreprocessing(string address)
        {
            address = address
                .Replace("«", "")
                .Replace("»", "")
                .Replace("\r", ",")
                .Replace("\n", ",")
                .Replace("\\r", ",")
                .Replace("\\n", ",");

            return address.Replace("\\", " ");
        }

        /// <summary>
        ///     Возвращает значение поля в текстовом виде.
        ///     Return field value.
        /// </summary>
        /// <param name="addrCodes">
        ///     Объект с кодами адреса.
        ///     Address codes data object.
        /// </param>
        /// <param name="settings">
        ///     Настройки построения адреса.
        ///     Build address settings.
        /// </param>
        /// <returns>
        ///     Построенное значение адреса в формате FIAS и в текстовом виде.
        ///     Field Value in Field Type Format.
        /// </returns>
        public AsAddressFieldValue BuildValueByCodes(AddrCodes addrCodes, BuildAddressSettings settings = null)
        {
            if (addrCodes == null)
                throw new ArgumentNullException(nameof(addrCodes));

            if (settings == null)
                settings = BuildAddressSettings.Default;

            using (var field = new AsAddressField(_scope))
            {
                AsAddressFieldValue fieldValue = field.BuildValue(addrCodes, settings);
                return fieldValue;
            }
        }

        /// <summary>
        ///     Разрешение города, либо населённого пункта.
        /// </summary>
        /// <param name="settlement">Город, либо населённый пункт для разрешения</param>
        /// <returns>
        ///     Возвращает:
        ///     Разрешённое значение города, либо населённого пункта в формате FIAS, вместе с кодами.
        /// </returns>
        public ResolveAddressStringResult ResolveSettlement(string settlement)
        {
            if (settlement.IsNullOrEmpty())
                return ResolveAddressStringResult.EmptyResult;

            var codes = NormalizeSettlement(settlement);
            var fieldValue = BuildValueByCodes(codes);
            return new ResolveAddressStringResult {
                Codes = codes,
                Result = fieldValue
            };
        }

        /// <summary>
        ///     Разрешение адреса.
        /// </summary>
        /// <param name="address">Адрес для разрешения</param>
        /// <param name="settings">Настройки нормализации. Null - значение настроек по-умолчанию.</param>
        /// <returns>
        ///     Возвращает:
        ///     Разрешённый адрес в формате FIAS, вместе с кодами.
        /// </returns>
        public ResolveAddressStringResult ResolveAddress(string address, NormalizeAddressSettings settings = null)
        {
            if (settings == null)
                settings = NormalizeAddressSettings.Default;

            if (address.IsNullOrEmpty())
                return ResolveAddressStringResult.EmptyResult;

            var buildSettings = MapNormalizeToBuildSettings(settings);

            var codes = NormalizeAddress(address, settings);
            var fieldValue = BuildValueByCodes(codes, buildSettings);
            return new ResolveAddressStringResult {
                Codes = codes,
                Result = fieldValue
            };
        }

        private BuildAddressSettings MapNormalizeToBuildSettings(NormalizeAddressSettings settings)
        {
            return new BuildAddressSettings {
                IncludeHouses = settings.IncludeHouses,
                HousesMatchMode = settings.HousesMatchMode
            };
        }

        /// <summary>
        ///      Сравнение адресов.
        /// </summary>
        /// <param name="one">Первый сравниваемый адрес</param>
        /// <param name="two">Второй сравниваемый адрес</param>
        /// <returns>
        ///     Возвращает:
        ///     True, если адреса эквивалентны
        ///     False, если нет
        /// </returns>
        public bool CompareAddressCodes(AddrCodes one, AddrCodes two)
        {
            return one.CityCode.PrepareToCompare() == two.CityCode.PrepareToCompare() &&
                   one.LocalityCode.PrepareToCompare() == two.LocalityCode.PrepareToCompare() &&
                   one.StreetCode.PrepareToCompare() == two.StreetCode.PrepareToCompare() &&
                   one.HouseCode.PrepareToCompare() == two.HouseCode.PrepareToCompare();
        }

        /// <summary>
        ///     Сравнение адресов.
        ///     Если все значения кодов - "пусто", то возвращает false
        /// </summary>
        /// <param name="one">Первый сравниваемый адрес</param>
        /// <param name="two">Второй сравниваемый адрес</param>
        /// <returns>
        ///     Возвращает:
        ///     True, если адреса эквивалентны
        ///     False, если нет, а также все значения кодов - "пусто"
        /// </returns>
        public bool CompareAddressCodesWithNullCheck(AddrCodes one, AddrCodes two)
        {
            var cityCodeOne = one.CityCode.PrepareToCompare();
            var cityCodeTwo = two.CityCode.PrepareToCompare();

            var localityCodeOne = one.LocalityCode.PrepareToCompare();
            var localityCodeTwo = two.LocalityCode.PrepareToCompare();

            var streetCodeOne = one.StreetCode.PrepareToCompare();
            var streetCodeTwo = two.StreetCode.PrepareToCompare();

            var houseCodeOne = one.HouseCode.PrepareToCompare();
            var houseCodeTwo = two.HouseCode.PrepareToCompare();

            //return (cityCodeOne == cityCodeTwo && !cityCodeOne.IsNullOrEmpty() && !cityCodeTwo.IsNullOrEmpty()) &&
            //       (localityCodeOne == localityCodeTwo) &&
            //       (streetCodeOne == streetCodeTwo && !streetCodeOne.IsNullOrEmpty() && !streetCodeTwo.IsNullOrEmpty());

            // Если оба адреса пусты, то они не эквивалентны
            if (one.IsEmpty() && two.IsEmpty())
                return false;

            return cityCodeOne == cityCodeTwo &&
                   localityCodeOne == localityCodeTwo &&
                   streetCodeOne == streetCodeTwo &&
                   houseCodeOne == houseCodeTwo;
        }

        /// <summary>
        ///     Возвращает значение поля в текстовом виде.
        ///     Return field value.
        /// </summary>
        /// <param name="fiasField">
        ///     Объект поля ФИАС.
        ///     FIAS field data object.
        /// </param>
        /// <returns>
        ///     Построенное значение адреса в формате FIAS и в текстовом виде.
        ///     Field Value in Field Type Format.
        /// </returns>
        public string BuildValue(FiasField fiasField)
        {
            if (fiasField == null)
                throw new ArgumentNullException(nameof(fiasField));

            using (var oper = new AsAddrObjsBOper(_scope))
            using (var field = new AsAddressField(_scope))
            {
                var addrCodes = ReadValueInternal(fiasField, oper);
                var fieldValue = field.BuildValue(addrCodes);
                return fieldValue.ToString();
            }
        }

        private AddrCodes ReadValueInternal(FiasField fiasField, AsAddrObjsBOper oper)
        {
            string regionCode = GetRegionCode(fiasField, oper);
            string districtCode = GetDistrictCode(fiasField, oper, ref regionCode);

            // Each city contain region or district
            string cityCode = GetCityCode(fiasField, oper, ref regionCode, ref districtCode);  
            string localityCode = GetLocalityCode(fiasField, oper, ref regionCode, ref districtCode, cityCode);

            string streetCode = string.Empty;
            Guid? houseId = null;
            if (!string.IsNullOrEmpty(fiasField.Street))
                streetCode = GetStreetCode(fiasField, oper, cityCode, localityCode);
            if (!string.IsNullOrEmpty(fiasField.House) && !string.IsNullOrEmpty(streetCode))
                houseId = GetHouseId(fiasField, oper, streetCode);

            AddrCodes addrCodes = new AddrCodes();
            addrCodes.RegionCode = regionCode;
            addrCodes.DistrictCode = districtCode;
            addrCodes.CityCode = cityCode;
            addrCodes.LocalityCode = localityCode;

            if (!string.IsNullOrEmpty(fiasField.Street))
                addrCodes.StreetCode = streetCode;
            if (!string.IsNullOrEmpty(fiasField.House) && !string.IsNullOrEmpty(streetCode))
                addrCodes.HouseCode = houseId.ToString();

            return addrCodes;
        }

        private string GetRegionCode(FiasField fiasField, AsAddrObjsBOper oper)
        {
            // The region may not be specified, it is not an error. Then the city will search for all regions
            var region = Convert.ToString(fiasField.Region).Trim();
            if (string.IsNullOrEmpty(region))
                return null;

            // Амурская область, Алтайский край, Башкортостан республика. The second word is not needed
            var array = region.Split(' ');
            AddrObjResponse regions = oper.GetRegions(null, 1, 2, array[0]);
            if (regions.Error != null)
                throw new InvalidOperationException($"{regions.Error.MessageError} {regions.Error.MessageForUser} {regions.Error.StackTrace}");

            if (regions.Items.Count > 1)
                throw new InvalidOperationException($"Существует не один регион с названием '{array[0]}'");

            if (regions.Items.Count == 0)
                throw new InvalidOperationException($"Не существует региона с названием '{array[0]}'");

            AddrObj regionItem = regions.Items[0];
            return regionItem.Code;
        }

        private string GetDistrictCode(FiasField fiasField, AsAddrObjsBOper oper, ref string regionCode)
        {
            // The area (district) may not be specified, it is not an error, the city can directly report to the region (for example, Самара - Самарская область)
            var district = Convert.ToString(fiasField.District).Trim();
            if (string.IsNullOrEmpty(district))
                return null;

            // Вельский р-н, Безенчукский р-н. The second word is not needed
            var array = district.Split(' ');
            AddrObjResponse districts = oper.GetDistricts(regionCode, null, 1, 2, array[0]);
            if (districts.Error != null)
                throw new InvalidOperationException($"{districts.Error.MessageError} {districts.Error.MessageForUser} {districts.Error.StackTrace}");

            if (districts.Items.Count > 1)
                throw new InvalidOperationException($"Существует не один район с названием  '{district}'");

            if (districts.Items.Count == 0)
                throw new InvalidOperationException($"Не существует района с названием '{district}'");

            if (!string.IsNullOrEmpty(districts.Items.FirstOrDefault()?.Region?.Code))
                regionCode = districts.Items.FirstOrDefault()?.Region?.Code;

            AddrObj districtItem = districts.Items[0];
            return districtItem.Code;
        }

        private string GetCityCode(FiasField fiasField, AsAddrObjsBOper oper, ref string regionCode, ref string districtCode)
        {
            // The city must always be given. Addresses without a city do not happen    
            
            var city = Convert.ToString(fiasField.City).Trim();
            if (city.IsNullOrEmpty())
                return null; //throw new InvalidOperationException("City must not be empty");

            city = city.Replace("г.", "").Trim();
            if (city.EndsWith(" г"))
            {
                city = city.Replace(" г", "").Trim();
            }

            AddrObjResponse cities = oper.GetCities(regionCode, districtCode, city, 1, 2, city);
            if (cities.Error != null)
                throw new InvalidOperationException($"{cities.Error.MessageError} {cities.Error.MessageForUser} {cities.Error.StackTrace}");

            if (cities.Items.Count > 1)
                throw new InvalidOperationException($"Существует не один город с названием '{city}'");

            if (cities.Items.Count == 0)
                throw new InvalidOperationException($"Не существует города с названием '{city}'");

            if (!string.IsNullOrEmpty(cities.Items.FirstOrDefault()?.Region?.Code))
                regionCode = cities.Items.FirstOrDefault()?.Region?.Code;

            if (!string.IsNullOrEmpty(cities.Items.FirstOrDefault()?.District?.Code))
                districtCode = cities.Items.FirstOrDefault()?.District?.Code;

            var cityItem = cities.Items.FirstOrDefault();
            return cityItem?.Code;
        }

        private string GetLocalityCode(FiasField fiasField, AsAddrObjsBOper oper, ref string regionCode, ref string districtCode, string cityCode)
        {
            // The settlement(locality - inside the city) may not be specified, it is not a mistake.The address is taken directly from the city         
            var locality = Convert.ToString(fiasField.Locality).Trim();

            if (regionCode.IsNullOrEmpty() && districtCode.IsNullOrEmpty())
                throw new InvalidOperationException("Если указывается населенный пункт, то ячейки с регионом и районом не должны быть одновременно пустыми.");
            if (locality.IsNullOrEmpty())
                return null;

            var localityTypes = new List<string> { " пгт", "с.", " поc", "пос.", "пгт.", " с", " п", "п.", " д", "д.", "ж/д_ст" };
            var currTypeShortName = String.Empty;

            foreach (var localityType in localityTypes)
            {
                if (locality.EndsWith(localityType))
                {
                    currTypeShortName = localityType.TrimEnd('.').Trim();
                    locality = locality.Replace(localityType, "").Trim();
                }
            }

            AddrObjResponse localities = oper.GetLocations(regionCode, districtCode, cityCode, null, 1, 2, locality);
            if (localities.Error != null)
                throw new InvalidOperationException($"{localities.Error.MessageError} {localities.Error.MessageForUser} {localities.Error.StackTrace}");

            if (localities.Items.Count > 1)
            {
                var localitesByTypeShortName = localities.Items.Where(x => x.TypeShortName == currTypeShortName).Select(x => x).ToList();
                if (localitesByTypeShortName.Count > 0)
                    localities.Items = localitesByTypeShortName;
            }

            if (localities.Items.Count > 1)
                throw new InvalidOperationException($"Существует не один населенный пункт с названием '{locality}'");

            if (localities.Items.Count == 0)
                throw new InvalidOperationException($"Не существует населённого пункта с названием '{locality}'");

            if (!string.IsNullOrEmpty(localities.Items.FirstOrDefault()?.Region?.Code))
                regionCode = localities.Items.FirstOrDefault()?.Region?.Code;

            if (!string.IsNullOrEmpty(localities.Items.FirstOrDefault()?.District?.Code))
                districtCode = localities.Items.FirstOrDefault()?.District?.Code;

            var localityItem = localities.Items[0];
            return localityItem.Code;
        }

        private string GetStreetCode(FiasField fiasField, AsAddrObjsBOper oper, string cityCode, string localityCode)
        {
            // Street format: [name] [type]
            // Type is street, area, avenue, highway, etc. (There are about 100 of them in the database)
            var street = Convert.ToString(fiasField.Street).Trim();
            if (street.IsNullOrEmpty())
                throw new InvalidOperationException("Адрес не должен быть пустым.");

            // The street may be subject to either the city or the locality. If the item is not specified, then the city is taken
            string parentCode = localityCode.IsNullOrEmpty() ? cityCode : localityCode;

            // We will assume that in a city there can not be more than 10 objects with the same name, but different types
            // We take everything and then check for the correct type
            var streetName = street.Substring(0, street.LastIndexOf(' ')).Trim();
            AddrObjResponse streets = oper.GetStreets(parentCode, null, 1, 10, streetName);
            if (streets.Error != null)
                throw new InvalidOperationException($"{streets.Error.MessageError} {streets.Error.MessageForUser} {streets.Error.StackTrace}");

            if (streets.Items.Count == 0)
                throw new InvalidOperationException($"Не существует улицы с названием '{streetName}'");

            var array = street.Split(' ');
            var streetType = array[array.Length - 1].Trim().ToLowerInvariant();
            var types  = oper.GetStreetTypes(streetType);

            if (types.Count == 0)
                throw new InvalidOperationException($"Не существует типа улицы с названием '{streetType}'");

            var streetItem = streets.Items.FirstOrDefault(_ => types.Any(x => _.TypeShortName.Equals(x.ShortName)));
            if (streetItem == null)
                throw new InvalidOperationException($"Не существует улицы с названием '{streetName}' и типом '{streetType}'");

            return streetItem.Code;
        }

        private Guid GetHouseId(FiasField fiasField, AsAddrObjsBOper oper, string parentCode)
        {
            var house = Convert.ToString(fiasField.House).Trim();
            if (house.IsNullOrEmpty())
                throw new InvalidOperationException("Номер дома не должен быть пустым.");

            var houseSearchTerm = house.Trim();
            if (houseSearchTerm.StartsWith("Дом"))
                houseSearchTerm = houseSearchTerm.Remove(0, 3).Trim();
            if (houseSearchTerm.StartsWith("дом"))
                houseSearchTerm = houseSearchTerm.Remove(0, 3).Trim();
            if (houseSearchTerm.StartsWith("д."))
                houseSearchTerm = houseSearchTerm.Remove(0, 2).Trim();
            if (houseSearchTerm.StartsWith("д"))
                houseSearchTerm = houseSearchTerm.Remove(0, 1).Trim();

            var houses = oper.GetHouses(parentCode, null, 1, 2, houseSearchTerm);
            if (houses.Error != null)
                throw new InvalidOperationException($"{houses.Error.MessageError} {houses.Error.MessageForUser} {houses.Error.StackTrace}");

            if (houses.Items.Count > 1)
                throw new InvalidOperationException($"Существует более одного дома с номером '{house}'");

            if (houses.Items.Count == 0)
                throw new InvalidOperationException($"Не существует дома с номером '{house}'");

            AddrObjHouse houseItem = houses.Items[0];
            return houseItem.HouseId;
        }

        private string TryGetRegionCode(string regionCandidate, AsAddrObjsBOper oper)
        {
            try
            {
                // The region may not be specified, it is not an error. Then the city will search for all regions
                var region = Convert.ToString(regionCandidate).Trim();
                if (string.IsNullOrEmpty(region))
                    return null;

                // Амурская область, Алтайский край, Башкортостан республика. The second word is not needed
                var array = region.Split(' ');
                AddrObjResponse regions = oper.GetRegions(null, 1, 2, array[0]);
                if (regions.Error != null)
                {
                    _log.Warning(
                        $"{regions.Error.MessageError} {regions.Error.MessageForUser} {regions.Error.StackTrace}");
                    return string.Empty;
                }

                if (regions.Items.Count > 1)
                {
                    _log.Warning($"Существует не один регион с названием '{array[0]}'");
                    return string.Empty;
                }

                if (regions.Items.Count == 0)
                {
                    _log.Warning($"Не существует региона с названием '{array[0]}'");
                    return string.Empty;
                }

                AddrObj regionItem = regions.Items[0];
                return regionItem.Code;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return string.Empty;
            }
        }

        private string TryGetDistrictCode(string districtCandidate, AsAddrObjsBOper oper, ref string regionCode)
        {
            try
            {
                // The area (district) may not be specified, it is not an error, the city can directly report to the region (for example, Самара - Самарская область)
                var district = Convert.ToString(districtCandidate).Trim();
                if (string.IsNullOrEmpty(district))
                    return null;

                // Вельский р-н, Безенчукский р-н. The second word is not needed
                var array = district.Split(' ');
                AddrObjResponse districts = oper.GetDistricts(regionCode, null, 1, 2, array[0]);
                if (districts.Error != null)
                {
                    _log.Warning(
                        $"{districts.Error.MessageError} {districts.Error.MessageForUser} {districts.Error.StackTrace}");
                    return string.Empty;
                }

                if (districts.Items.Count > 1)
                {
                    _log.Warning($"Существует не один район с названием  '{district}'");
                    return string.Empty;
                }

                if (districts.Items.Count == 0)
                {
                    _log.Warning($"Не существует района с названием '{district}'");
                    return string.Empty;
                }

                if (!string.IsNullOrEmpty(districts.Items.FirstOrDefault()?.Region?.Code))
                    regionCode = districts.Items.FirstOrDefault()?.Region?.Code;

                AddrObj districtItem = districts.Items[0];
                return districtItem.Code;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return string.Empty;
            }
        }

        private string TryGetCityCode(string cityCandidate, AsAddrObjsBOper oper, ref string regionCode, ref string districtCode)
        {
            try
            {
                // The city must always be given. Addresses without a city do not happen    
                var city = Convert.ToString(cityCandidate).Trim();
                if (city.IsNullOrEmpty())
                    return null; //throw new InvalidOperationException("City must not be empty");

                city = city.Replace("г.", "").Trim();
                if (city.EndsWith(" г"))
                {
                    city = city.Replace(" г", "").Trim();
                }

                AddrObjResponse cities = oper.GetCities(regionCode, districtCode, city, 1, 2, city);
                if (cities.Error != null)
                {
                    _log.Warning(
                        $"{cities.Error.MessageError} {cities.Error.MessageForUser} {cities.Error.StackTrace}");
                    return string.Empty;
                }

                if (cities.Items.Count > 1)
                {
                    _log.Warning($"Существует не один город с названием '{city}'");
                    return string.Empty;
                }

                if (cities.Items.Count == 0)
                {
                    _log.Warning($"Не существует города с названием '{city}'");
                    return string.Empty;
                }

                if (!string.IsNullOrEmpty(cities.Items.FirstOrDefault()?.Region?.Code))
                    regionCode = cities.Items.FirstOrDefault()?.Region?.Code;

                if (!string.IsNullOrEmpty(cities.Items.FirstOrDefault()?.District?.Code))
                    districtCode = cities.Items.FirstOrDefault()?.District?.Code;

                var cityItem = cities.Items.FirstOrDefault();
                return cityItem?.Code;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return string.Empty;
            }
        }

        private string TryGetLocalityCode(
            string localityCandidate, 
            AsAddrObjsBOper oper,
            ref string regionCode,
            ref string districtCode, 
            string cityCode)
        {
            try
            {
                // The settlement(locality - inside the city) may not be specified, it is not a mistake.The address is taken directly from the city         
                var locality = Convert.ToString(localityCandidate).Trim();

                if (regionCode.IsNullOrEmpty() && districtCode.IsNullOrEmpty())
                    throw new InvalidOperationException(
                        "Если указывается населенный пункт, то ячейки с регионом и районом не должны быть одновременно пустыми.");
                if (locality.IsNullOrEmpty())
                    return null;

                var localityTypes = new List<string>
                    {" пгт", "с.", " поc", "пос.", "пгт.", " с", " п", "п.", " д", "д.", "ж/д_ст"};
                var currTypeShortName = String.Empty;

                foreach (var localityType in localityTypes)
                {
                    if (locality.EndsWith(localityType))
                    {
                        currTypeShortName = localityType.TrimEnd('.').Trim();
                        locality = locality.Replace(localityType, "").Trim();
                    }
                }

                AddrObjResponse localities =
                    oper.GetLocations(regionCode, districtCode, cityCode, null, 1, 2, locality);
                if (localities.Error != null)
                    throw new InvalidOperationException(
                        $"{localities.Error.MessageError} {localities.Error.MessageForUser} {localities.Error.StackTrace}");

                if (localities.Items.Count > 1)
                {
                    var localitesByTypeShortName = localities.Items.Where(x => x.TypeShortName == currTypeShortName)
                        .Select(x => x).ToList();
                    if (localitesByTypeShortName.Count > 0)
                        localities.Items = localitesByTypeShortName;
                }

                if (localities.Items.Count > 1)
                    throw new InvalidOperationException(
                        $"Существует не один населенный пункт с названием '{locality}'");

                if (localities.Items.Count == 0)
                    throw new InvalidOperationException($"Не существует населённого пункта с названием '{locality}'");

                if (!string.IsNullOrEmpty(localities.Items.FirstOrDefault()?.Region?.Code))
                    regionCode = localities.Items.FirstOrDefault()?.Region?.Code;

                if (!string.IsNullOrEmpty(localities.Items.FirstOrDefault()?.District?.Code))
                    districtCode = localities.Items.FirstOrDefault()?.District?.Code;

                var localityItem = localities.Items.FirstOrDefault();
                return localityItem?.Code;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return string.Empty;
            }
        }

        private string TryGetStreetCode(string streetCandidate, AsAddrObjsBOper oper, string cityCode, string localityCode)
        {
            try
            {
                // Street format: [name] [type]
                // Type is street, area, avenue, highway, etc. (There are about 100 of them in the database)
                var street = Convert.ToString(streetCandidate).Trim();
                if (street.IsNullOrEmpty())
                {
                    _log.Warning("Адрес не должен быть пустым.");
                    return string.Empty;
                }

                // The street may be subject to either the city or the locality. If the item is not specified, then the city is taken
                string parentCode = localityCode.IsNullOrEmpty() ? cityCode : localityCode;

                // We will assume that in a city there can not be more than 10 objects with the same name, but different types
                // We take everything and then check for the correct type
                var streetCandidates = street.Split(' ');
                AddrObjResponse streets = new AddrObjResponse();
                foreach (var part in streetCandidates)
                {
                    //var streetName = street.Substring(0, street.LastIndexOf(' ')).Trim();
                    var streetName = part;
                    streets = oper.GetStreets(parentCode, null, 1, 10, streetName);
                    if (streets.Error != null)
                        _log.Warning($"{streets.Error.MessageError} {streets.Error.MessageForUser} {streets.Error.StackTrace}");

                    if (streets.Items.Count > 0)
                        break;
                }

                if (streets.Items.Count == 0)
                {
                    _log.Warning($"Не существует улицы с названием '{streetCandidate}'");
                    return string.Empty;
                }

                //var array = street.Split(' ');
                //var streetType = array[array.Length - 1].Trim().ToLowerInvariant();

                List<AsAddrObjType> types;
                AddrObj streetItem = null;
                var typeCandidates = street.Split(' ');
                foreach (var typeCandidate in typeCandidates)
                {
                    var streetType = typeCandidate.Trim().ToLowerInvariant();
                    types = oper.GetStreetTypes(streetType);

                    if (types.Count == 0)
                        _log.Warning($"Не существует типа улицы с названием '{streetType}'");

                    streetItem = streets.Items.FirstOrDefault(_ => types.Any(x => _.TypeShortName.Equals(x.ShortName)));
                    if (streetItem == null)
                        _log.Warning($"Не существует улицы с названием '{streetCandidate}' и типом '{streetType}'");
                    else
                        break;
                }

                return streetItem?.Code;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return string.Empty;
            }
        }

        private Guid? TryGetHouseId(string houseCandidate, AsAddrObjsBOper oper, string parentCode)
        {
            try
            {
                var house = Convert.ToString(houseCandidate).Trim();
                if (house.IsNullOrEmpty())
                    throw new InvalidOperationException("Номер дома не должен быть пустым.");

                var houseSearchTerm = house.Trim();
                if (houseSearchTerm.StartsWith("Дом"))
                    houseSearchTerm = houseSearchTerm.Remove(0, 3).Trim();
                if (houseSearchTerm.StartsWith("дом"))
                    houseSearchTerm = houseSearchTerm.Remove(0, 3).Trim();
                if (houseSearchTerm.StartsWith("д."))
                    houseSearchTerm = houseSearchTerm.Remove(0, 2).Trim();
                if (houseSearchTerm.StartsWith("д"))
                    houseSearchTerm = houseSearchTerm.Remove(0, 1).Trim();

                var houses = oper.GetHouses(parentCode, null, 1, 2, houseSearchTerm);
                if (houses.Error != null)
                {
                    _log.Warning(
                        $"{houses.Error.MessageError} {houses.Error.MessageForUser} {houses.Error.StackTrace}");
                    return null;
                }

                if (houses.Items.Count > 1)
                {
                    _log.Warning($"Существует более одного дома с номером '{house}'");
                    return null;
                }

                if (houses.Items.Count == 0)
                {
                    _log.Warning($"Не существует дома с номером '{house}'");
                    return null;
                }

                AddrObjHouse houseItem = houses.Items[0];
                return houseItem.HouseId;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return null;
            }
        }

        private string TryGetRegionCodeMultiple(
            AsAddrObjsBOper oper,
            bool extendSearchPatterns,
            params string[] regionCandidates)
        {
            try
            {
                // The region may not be specified, it is not an error. Then the city will search for all regions
                var regionCand = regionCandidates.Where(x => !x.Trim().IsNullOrEmpty()).ToArray();
                if (regionCand.Length == 0)
                {
                    return string.Empty;
                }

                // Амурская область, Алтайский край, Башкортостан республика. The second word is not needed
                var regionNames = new MatList<string>();
                foreach (var regionCandidate in regionCand)
                {
                    var regionName = regionCandidate;

                    // Извлечение региона без типа
                    regionNames += _fiasUtil.GetRegionWithoutType(regionCandidate);

                    if (extendSearchPatterns)
                    {
                        // Добавляем название необработанное
                        var nameRaw = regionName;
                        regionNames += nameRaw.Trim();

                        // Если includeSplitOnPartNames == true, то
                        // дополнительно включаем название региона поделённое на части.
                        // т.к это увеличит охват
                        var parts = regionName.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToMatList();
                        regionNames += parts;
                    }
                }

                AddrObjResponse regions = oper.GetRegionsMultiple(null, 1, 2, regionNames.ToArray());
                if (regions.Error != null)
                {
                    _log.Warning(
                        $"{regions.Error.MessageError} {regions.Error.MessageForUser} {regions.Error.StackTrace}");
                    return string.Empty;
                }

                if (regions.Items.Count == 0)
                {
                    var rs = string.Join(", ", regionNames);
                    _log.Warning($"Не существует регионов с возможными названиями '{rs}'");
                    return string.Empty;
                }

                if (regions.Items.Count > 1)
                {
                    var rs = string.Join(", ", regionNames);
                    var fstItem = regions.Items.FirstOrDefault();
                    _log.Warning($"Существует не один регион с возможными названиями '{rs}'. Возвращается первый регион с названием '{fstItem?.Name}' и кодом '{fstItem?.Code}'.");
                    return fstItem?.Code;
                }

                AddrObj regionItem = regions.Items[0];
                return regionItem.Code;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return string.Empty;
            }
        }

        private string TryGetDistrictCodeMiltiple(
            AsAddrObjsBOper oper,
            ref string regionCode,
            bool extendSearchPatterns,
            params string[] districtCandidates)
        {
            try
            {
                // The area (district) may not be specified, it is not an error, the city can directly report to the region (for example, Самара - Самарская область)
                var districtCand = districtCandidates.Where(x => !x.Trim().IsNullOrEmpty()).ToArray();
                if (districtCand.Length == 0)
                {
                    return string.Empty;
                }

                // Вельский р-н, Безенчукский р-н. The second word is not needed
                var districtNames = new MatList<string>();
                foreach (var districtCandidate in districtCand)
                {
                    var districtName = districtCandidate;

                    // Извлечение района без типа
                    districtNames += _fiasUtil.GetDistrictWithoutType(districtCandidate);

                    if (extendSearchPatterns)
                    {
                        // Добавляем название необработанное
                        var nameRaw = districtName;
                        districtNames += nameRaw.Trim();

                        // Если includeSplitOnPartNames == true, то
                        // дополнительно включаем название района поделённое на части.
                        // т.к это увеличит охват
                        var parts = districtName.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToMatList();
                        districtNames += parts;
                    }
                }

                AddrObjResponse districts = oper.GetDistrictsMultiple(regionCode, null, 1, 2, districtNames.ToArray());
                if (districts.Error != null)
                {
                    _log.Warning(
                        $"{districts.Error.MessageError} {districts.Error.MessageForUser} {districts.Error.StackTrace}");
                    return string.Empty;
                }

                if (districts.Items.Count == 0)
                {
                    var ds = string.Join(", ", districtNames);
                    _log.Warning($"Не существует районов с возможными названиями '{ds}'");
                    return string.Empty;
                }

                if (districts.Items.Count > 1)
                {
                    var ds = string.Join(", ", districtNames);
                    var fstItem = districts.Items.FirstOrDefault();
                    _log.Warning($"Существует не один район с возможными названиями '{ds}'. Возвращается первый район с названием '{fstItem?.Name}' и кодом '{fstItem?.Code}'.");
                    return fstItem?.Code;
                }

                if (!string.IsNullOrEmpty(districts.Items.FirstOrDefault()?.Region?.Code))
                    regionCode = districts.Items.FirstOrDefault()?.Region?.Code;

                AddrObj districtItem = districts.Items[0];
                return districtItem.Code;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return string.Empty;
            }
        }

        private string TryGetCityCodeMultiple(
            AsAddrObjsBOper oper,
            ref string regionCode,
            ref string districtCode,
            bool extendSearchPatterns,
            params string[] cityCandidates)
        {
            try
            {
                // The city must always be given. Addresses without a city do not happen    
                var cityCand = cityCandidates.Where(x => !x.Trim().IsNullOrEmpty()).ToArray();
                if (cityCand.Length == 0)
                {
                    return string.Empty;
                }

                var cityNames = new MatList<string>();
                for (var index = 0; index < cityCand.Length; index++)
                {
                    var cityName = cityCand[index];

                    // Извлечение города без типа
                    cityName = _fiasUtil.GetCityWithoutType(cityName);
                    cityNames += cityName;

                    if (extendSearchPatterns)
                    {
                        // Добавляем название необработанное
                        var nameRaw = cityName;
                        cityNames += nameRaw.Trim();

                        // Если includeSplitOnPartNames == true, то
                        // дополнительно включаем название города поделённое на части.
                        // т.к это увеличит охват
                        var parts = cityName.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToMatList();
                        cityNames += parts;
                    }
                }

                // TODO -> возможно стоит пробросить searchTerm в виде cityNames
                AddrObjResponse cities = oper.GetCitiesMultiple(regionCode, districtCode, null, 1, 2, cityNames.ToArray());
                if (cities.Error != null)
                {
                    _log.Warning(
                        $"{cities.Error.MessageError} {cities.Error.MessageForUser} {cities.Error.StackTrace}");
                    return string.Empty;
                }

                if (cities.Items.Count == 0)
                {
                    var cs = string.Join(", ", cityNames);
                    _log.Warning($"Не существует городов с возможными названиями '{cs}'");
                    return string.Empty;
                }

                if (cities.Items.Count > 1)
                {
                    var cs = string.Join(", ", cityNames);
                    var fstItem = cities.Items.FirstOrDefault();
                    _log.Warning($"Существует не один город с возможными названиями '{cs}'. Возвращается первый город с названием '{fstItem?.Name}' и кодом '{fstItem?.Code}'.");
                    return fstItem?.Code;
                }

                if (!string.IsNullOrEmpty(cities.Items.FirstOrDefault()?.Region?.Code))
                    regionCode = cities.Items.FirstOrDefault()?.Region?.Code;

                if (!string.IsNullOrEmpty(cities.Items.FirstOrDefault()?.District?.Code))
                    districtCode = cities.Items.FirstOrDefault()?.District?.Code;

                var cityItem = cities.Items.FirstOrDefault();
                return cityItem?.Code;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return string.Empty;
            }
        }

        private string TryGetLocalityCodeMultiple(
            AsAddrObjsBOper oper,
            ref string regionCode,
            ref string districtCode,
            string cityCode,
            params string[] localityCandidates)
        {
            try
            {
                // The settlement(locality - inside the city) may not be specified, it is not a mistake.The address is taken directly from the city         
                var localityCand = localityCandidates.Where(x => !x.Trim().IsNullOrEmpty()).ToArray();
                if (localityCand.Length == 0)
                {
                    return string.Empty;
                }

                if (regionCode.IsNullOrEmpty() && districtCode.IsNullOrEmpty())
                    throw new InvalidOperationException(
                        "Если указывается населенный пункт, то ячейки с регионом и районом не должны быть одновременно пустыми.");

                var localityNames = new MatList<string>();
                foreach (var locality in localityCand)
                {
                    // Добавляем название необработанное
                    var localityNameRaw = locality;
                    localityNames += localityNameRaw.Trim();

                    // Добавляем возможное название без типа

                    // Извлечение населённого пункта без типа
                    var localityName = _fiasUtil.GetLocationWithoutType(locality);
                    localityNames += localityName;

                    // Дополнительно включаем название улицы поделённое на части.
                    // т.к это увеличит охват
                    var locationNameParts = locality.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToMatList();
                    localityNames += locationNameParts;
                }

                AddrObjResponse localities =
                    oper.GetLocationsMultiple(regionCode, districtCode, cityCode, null, 1, 2, localityNames.ToArray());
                if (localities.Error != null)
                    throw new InvalidOperationException(
                        $"{localities.Error.MessageError} {localities.Error.MessageForUser} {localities.Error.StackTrace}");

                if (localities.Items.Count == 0)
                {
                    var ls = string.Join(", ", localityNames);
                    _log.Warning($"Не существует населённых пунктов с возможными названиями '{ls}'");
                }

                if (localities.Items.Count > 1)
                {
                    var ls = string.Join(", ", localityNames);
                    _log.Warning(
                        $"Существует не один населенный пункт с возможными названиями '{ls}'");
                }

                if (!string.IsNullOrEmpty(localities.Items.FirstOrDefault()?.Region?.Code))
                    regionCode = localities.Items.FirstOrDefault()?.Region?.Code;

                if (!string.IsNullOrEmpty(localities.Items.FirstOrDefault()?.District?.Code))
                    districtCode = localities.Items.FirstOrDefault()?.District?.Code;

                var localityItem = localities.Items.FirstOrDefault();
                return localityItem?.Code;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return string.Empty;
            }
        }

        private string TryGetStreetCodeMultiple(AsAddrObjsBOper oper, string cityCode, string localityCode, bool actualIgnore = false, params string[] streetCandidates)
        {
            try
            {
                // Street format: [name] [type]
                // Type is street, area, avenue, highway, etc. (There are about 100 of them in the database)
                var streetCand = streetCandidates.Where(x => !x.Trim().IsNullOrEmpty()).ToArray();
                if (streetCand.Length == 0)
                {
                    _log.Warning("Адрес не должен быть пустым.");
                    return string.Empty;
                }

                // The street may be subject to either the city or the locality. If the item is not specified, then the city is taken
                string parentCode = localityCode.IsNullOrEmpty() ? cityCode : localityCode;

                // Если и город и населённый пункт заполнены, то берём название населённого пункта
                if (!cityCode.IsNullOrEmpty() && !localityCode.IsNullOrEmpty())
                    parentCode = localityCode;

                // We will assume that in a city there can not be more than 10 objects with the same name, but different types
                // We take everything and then check for the correct type

                var streetNames = new MatList<string>();
                foreach (var street in streetCand)
                {
                    // Добавляем название необработанное
                    var streetNameRaw = street;
                    streetNames += streetNameRaw.Trim();

                    // Добавляем возможное название без типа
                    var streetName = _fiasUtil.GetStreetWithoutType(street);
                    streetNames += streetName;

                    // Дополнительно включаем название улицы поделённое на части.
                    // т.к это увеличит охват
                    var streetNameParts = streetName.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToMatList();
                    streetNames += streetNameParts;

                    // Добавляем также фрагменты скомбинориванные парами
                    // Требуется для разрешения проблемы улиу из двух слов
                    streetNames += ReassemblyStringAsPairs(street);

                    streetNames = streetNames.Distinct().ToMatList();
                }

                var streets = oper.GetStreetsMultiple(parentCode, null, 1, 10, actualIgnore, streetNames.ToArray());
                if (streets.Error != null)
                    _log.Warning($"{streets.Error.MessageError} {streets.Error.MessageForUser} {streets.Error.StackTrace}");

                if (streets.Items.Count == 0)
                {
                    var strs = string.Join(", ", streetCand);
                    _log.Warning($"Не существует улиц с возможными названиями '{strs}'");
                    return string.Empty;
                }

                var streetTypeNames = new MatList<string>();
                foreach (var street in streetCand)
                {
                    var typeCandidates = street.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var typeCandidatesMat = new MatList<string>();

                    foreach (var typeCandidate in typeCandidates)
                    {
                        var streetType = typeCandidate.Trim().ToLowerInvariant();

                        // Приведение типа объекта адреса (множественное)
                        var streetTypes = _fiasUtil.ResolveTypes(streetType).Select(x => x.ToLowerInvariant()).ToMatList();
                        typeCandidatesMat += streetTypes;
                    }
                    streetTypeNames += typeCandidatesMat.ToMatList();
                }

                var types = oper.GetStreetTypesMultiple(streetTypeNames.ToArray());

                if (types.Count == 0)
                {
                    var streetTypeNamesSfy = string.Join(", ", streetTypeNames);
                    _log.Warning($"Не существует типа улицы с названиями '{streetTypeNamesSfy}'");
                    return string.Empty;
                }

                var streetItem = streets.Items.FirstOrDefault(_ => types.Any(x => _.TypeShortName.Equals(x.ShortName)));
                if (streetItem == null)
                {
                    var streetNamesSfy = string.Join(", ", streetNames);
                    var streetTypeNamesSfy = string.Join(", ", streetTypeNames);
                    _log.Warning($"Не существует улицы с названиями '{streetNamesSfy}' и типами '{streetTypeNamesSfy}'");
                    return string.Empty;
                }

                return streetItem?.Code;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return string.Empty;
            }
        }

        private MatList<string> ReassemblyStringAsPairs(string streetCandidate)
        {
            var streetNameParts = streetCandidate.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToMatList();

            var combinedFragmentsToPairs = new MatList<string>();
            for (int i = 0; i < streetNameParts.Count - 1; i++)
            {
                var current = streetNameParts[i];
                var next = streetNameParts[i + 1];

                var pair = $"{current} {next}";
                combinedFragmentsToPairs += pair;
            }

            return combinedFragmentsToPairs;
        }

        private string[] TryGetStreetCodesMultiple(AsAddrObjsBOper oper, string cityCode, string localityCode, bool actualIgnore = false, params string[] streetCandidates)
        {
            try
            {
                // Street format: [name] [type]
                // Type is street, area, avenue, highway, etc. (There are about 100 of them in the database)
                var streetCand = streetCandidates.Where(x => !x.Trim().IsNullOrEmpty()).ToArray();
                if (streetCand.Length == 0)
                {
                    _log.Warning("Адрес не должен быть пустым.");
                    return new string[] { };
                }

                // The street may be subject to either the city or the locality. If the item is not specified, then the city is taken
                string parentCode = localityCode.IsNullOrEmpty() ? cityCode : localityCode;

                // Если и город и населённый пункт заполнены, то берём название населённого пункта
                if (!cityCode.IsNullOrEmpty() && !localityCode.IsNullOrEmpty())
                    parentCode = localityCode;

                // We will assume that in a city there can not be more than 10 objects with the same name, but different types
                // We take everything and then check for the correct type

                var streetNames = new MatList<string>();
                foreach (var street in streetCand)
                {
                    // Добавляем название необработанное
                    var streetNameRaw = street;
                    streetNames += streetNameRaw.Trim();

                    // Добавляем возможное название без типа
                    var streetName = _fiasUtil.GetStreetWithoutType(street);
                    streetNames += streetName;

                    // Дополнительно включаем название улицы поделённое на части.
                    // т.к это увеличит охват
                    var streetNameParts = streetName.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToMatList();
                    streetNames += streetNameParts;
                }

                var streets = oper.GetStreetsMultiple(parentCode, null, 1, 100, actualIgnore, streetNames.ToArray());
                if (streets.Error != null)
                    _log.Warning($"{streets.Error.MessageError} {streets.Error.MessageForUser} {streets.Error.StackTrace}");

                if (streets.Items.Count == 0)
                {
                    var strs = string.Join(", ", streetCand);
                    _log.Warning($"Не существует улиц с возможными названиями '{strs}'");
                    return new string[] { };
                }

                var streetTypeNames = new MatList<string>();
                foreach (var street in streetCand)
                {
                    var typeCandidates = street.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var typeCandidatesMat = new MatList<string>();

                    foreach (var typeCandidate in typeCandidates)
                    {
                        var streetType = typeCandidate.Trim().ToLowerInvariant();

                        // Приведение типа объекта адреса (множественное)
                        var streetTypes = _fiasUtil.ResolveTypes(streetType).Select(x => x.ToLowerInvariant()).ToMatList();
                        typeCandidatesMat += streetTypes;
                    }
                    streetTypeNames += typeCandidatesMat.ToMatList();
                }

                var types = oper.GetStreetTypesMultiple(streetTypeNames.ToArray());

                if (types.Count == 0)
                {
                    var streetTypeNamesSfy = string.Join(", ", streetTypeNames);
                    _log.Warning($"Не существует типа улицы с названиями '{streetTypeNamesSfy}'");
                    return new string[] { };
                }

                var streetItems = streets.Items.Where(_ => types.Any(x => _.TypeShortName.Equals(x.ShortName))).ToList();
                if (streetItems.Count == 0)
                {
                    var streetNamesSfy = string.Join(", ", streetNames);
                    var streetTypeNamesSfy = string.Join(", ", streetTypeNames);
                    _log.Warning($"Не существует улицы с названиями '{streetNamesSfy}' и типами '{streetTypeNamesSfy}'");
                    return new string[] { };
                }

                return streetItems.Select(x => x.Code).ToArray();
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return new string[] { };
            }
        }

        /// <summary>
        ///     Получение улицы без учёта родительского адресного элемента будь то город, либо населённый пункт
        /// </summary>
        /// <param name="oper"></param>
        /// <param name="streetCandidates"></param>
        /// <returns></returns>
        private string TryGetStreetByOnlyStreetNamesCodeMultiple(AsAddrObjsBOper oper, params string[] streetCandidates)
        {
            try
            {
                // Street format: [name] [type]
                // Type is street, area, avenue, highway, etc. (There are about 100 of them in the database)
                var streetCand = streetCandidates.Where(x => !x.Trim().IsNullOrEmpty()).ToArray();
                if (streetCand.Length == 0)
                {
                    _log.Warning("Адрес не должен быть пустым.");
                    return string.Empty;
                }

                // We will assume that in a city there can not be more than 10 objects with the same name, but different types
                // We take everything and then check for the correct type

                var streetNames = new MatList<string>();
                foreach (var street in streetCand)
                {
                    // Добавляем название необработанное
                    var streetNameRaw = street;
                    streetNames += streetNameRaw.Trim();

                    // Добавляем возможное название без типа
                    var streetName = _fiasUtil.GetStreetWithoutType(street);
                    streetNames += streetName;

                    // Дополнительно включаем название улицы поделённое на части.
                    // т.к это увеличит охват
                    var streetNameParts = streetName.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToMatList();
                    streetNames += streetNameParts;
                }

                var streets = oper.GetStreetsByOnlyStreetNamesMultiple(null, 1, 100, streetNames.ToArray());
                if (streets.Error != null)
                    _log.Warning($"{streets.Error.MessageError} {streets.Error.MessageForUser} {streets.Error.StackTrace}");

                if (streets.Items.Count == 0)
                {
                    var strs = string.Join(", ", streetCand);
                    _log.Warning($"Не существует улиц с возможными названиями '{strs}'");
                    return string.Empty;
                }

                var streetTypeNames = new MatList<string>();
                foreach (var street in streetCand)
                {
                    var typeCandidates = street.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var typeCandidatesMat = new MatList<string>();

                    foreach (var typeCandidate in typeCandidates)
                    {
                        var streetType = typeCandidate.Trim().ToLowerInvariant();

                        // Приведение типа объекта адреса (множественное)
                        var streetTypes = _fiasUtil.ResolveTypes(streetType).Select(x => x.ToLowerInvariant()).ToMatList();
                        typeCandidatesMat += streetTypes;
                    }
                    streetTypeNames += typeCandidatesMat.ToMatList();
                }

                var types = oper.GetStreetTypesMultiple(streetTypeNames.ToArray());

                if (types.Count == 0)
                {
                    var streetTypeNamesSfy = string.Join(", ", streetTypeNames);
                    _log.Warning($"Не существует типа улицы с названиями '{streetTypeNamesSfy}'");
                    return string.Empty;
                }

                var streetItem = streets.Items.FirstOrDefault(_ => types.Any(x => _.TypeShortName.Equals(x.ShortName)));
                if (streetItem == null)
                {
                    var streetNamesSfy = string.Join(", ", streetNames);
                    var streetTypeNamesSfy = string.Join(", ", streetTypeNames);
                    _log.Warning($"Не существует улицы с названиями '{streetNamesSfy}' и типами '{streetTypeNamesSfy}'");
                    return string.Empty;
                }

                return streetItem?.Code;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return string.Empty;
            }
        }

        private Guid? TryGetHouseIdMultiple(AsAddrObjsBOper oper, string parentCode, params string[] houseCandidates)
        {
            try
            {
                var housesCand = houseCandidates.Where(x => !x.Trim().IsNullOrEmpty()).ToArray();
                if (housesCand.Length == 0)
                    throw new InvalidOperationException("Номер дома не должен быть пустым.");

                var houseSearchTerms = new MatList<string>();
                foreach (var house in housesCand)
                {
                    var houseSearchTerm = house.Trim();
                    if (houseSearchTerm.StartsWith("Дом"))
                        houseSearchTerm = houseSearchTerm.Remove(0, 3).Trim();
                    if (houseSearchTerm.StartsWith("дом"))
                        houseSearchTerm = houseSearchTerm.Remove(0, 3).Trim();
                    if (houseSearchTerm.StartsWith("д."))
                        houseSearchTerm = houseSearchTerm.Remove(0, 2).Trim();
                    if (houseSearchTerm.StartsWith("д"))
                        houseSearchTerm = houseSearchTerm.Remove(0, 1).Trim();

                    houseSearchTerms += houseSearchTerm;
                }

                var houses = oper.GetHousesMultiple(parentCode, null, onlyOnePage: true, houseSearchTerms.ToArray());
                if (houses.Error != null)
                {
                    _log.Warning(
                        $"{houses.Error.MessageError} {houses.Error.MessageForUser} {houses.Error.StackTrace}");
                    return null;
                }

                if (houses.Items.Count == 0)
                {
                    var housesNum = string.Join(", ", housesCand);
                    _log.Warning($"Не существует домов с возможными номерами '{housesNum}'");
                    return null;
                }

                if (houses.Items.Count > 1)
                {
                    var housesNum = string.Join(", ", housesCand);
                    var fstItem = houses.Items.FirstOrDefault();
                    _log.Warning($"Существует более одного дома с возможными номерами '{housesNum}'. Возвращается первый дом с номером '{fstItem?.HouseNum}' и ИД '{fstItem?.HouseId}'.");
                    return fstItem?.HouseId;
                }

                AddrObjHouse houseItem = houses.Items.FirstOrDefault();
                return houseItem?.HouseId;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return null;
            }
        }

        private Guid? TryGetHouseIdMultiple(AsAddrObjsBOper oper, string[] parentCodes, params string[] houseCandidates)
        {
            try
            {
                var housesCand = houseCandidates.Where(x => !x.Trim().IsNullOrEmpty()).ToArray();
                if (housesCand.Length == 0)
                    throw new InvalidOperationException("Номер дома не должен быть пустым.");

                var houseSearchTerms = new MatList<string>();
                foreach (var house in housesCand)
                {
                    var houseSearchTerm = house.Trim();
                    if (houseSearchTerm.StartsWith("Дом"))
                        houseSearchTerm = houseSearchTerm.Remove(0, 3).Trim();
                    if (houseSearchTerm.StartsWith("дом"))
                        houseSearchTerm = houseSearchTerm.Remove(0, 3).Trim();
                    if (houseSearchTerm.StartsWith("д."))
                        houseSearchTerm = houseSearchTerm.Remove(0, 2).Trim();
                    if (houseSearchTerm.StartsWith("д"))
                        houseSearchTerm = houseSearchTerm.Remove(0, 1).Trim();

                    houseSearchTerms += houseSearchTerm;
                }

                var houses = oper.GetHousesMultiple(parentCodes, null, onlyOnePage: true, houseSearchTerms.ToArray());
                if (houses.Error != null)
                {
                    _log.Warning(
                        $"{houses.Error.MessageError} {houses.Error.MessageForUser} {houses.Error.StackTrace}");
                    return null;
                }

                if (houses.Items.Count == 0)
                {
                    var housesNum = string.Join(", ", housesCand);
                    _log.Warning($"Не существует домов с возможными номерами '{housesNum}'");
                    return null;
                }

                if (houses.Items.Count > 1)
                {
                    var housesNum = string.Join(", ", housesCand);
                    var fstItem = houses.Items.FirstOrDefault();
                    _log.Warning($"Существует более одного дома с возможными номерами '{housesNum}'. Возвращается первый дом с номером '{fstItem?.HouseNum}' и ИД '{fstItem?.HouseId}'.");
                    return fstItem?.HouseId;
                }

                AddrObjHouse houseItem = houses.Items.FirstOrDefault();
                return houseItem?.HouseId;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return null;
            }
        }

        private Guid? TryGetHouseIdByOnlyHouseNumberMultiple(AsAddrObjsBOper oper, params string[] houseCandidates)
        {
            try
            {
                var housesCand = houseCandidates.Where(x => !x.Trim().IsNullOrEmpty()).ToArray();
                if (housesCand.Length == 0)
                    throw new InvalidOperationException("Номер дома не должен быть пустым.");

                var houseSearchTerms = new MatList<string>();
                foreach (var house in housesCand)
                {
                    var houseSearchTerm = house.Trim();
                    if (houseSearchTerm.StartsWith("Дом"))
                        houseSearchTerm = houseSearchTerm.Remove(0, 3).Trim();
                    if (houseSearchTerm.StartsWith("дом"))
                        houseSearchTerm = houseSearchTerm.Remove(0, 3).Trim();
                    if (houseSearchTerm.StartsWith("д."))
                        houseSearchTerm = houseSearchTerm.Remove(0, 2).Trim();
                    if (houseSearchTerm.StartsWith("д"))
                        houseSearchTerm = houseSearchTerm.Remove(0, 1).Trim();

                    houseSearchTerms += houseSearchTerm;
                }

                var houses = oper.GetHousesByOnlyHouseNumberMultiple(null, onlyOnePage: true, houseSearchTerms.ToArray());
                if (houses.Error != null)
                {
                    _log.Warning(
                        $"{houses.Error.MessageError} {houses.Error.MessageForUser} {houses.Error.StackTrace}");
                    return null;
                }

                if (houses.Items.Count == 0)
                {
                    var housesNum = string.Join(", ", housesCand);
                    _log.Warning($"Не существует домов с возможными номерами '{housesNum}'");
                    return null;
                }

                if (houses.Items.Count > 1)
                {
                    var housesNum = string.Join(", ", housesCand);
                    var fstItem = houses.Items.FirstOrDefault();
                    _log.Warning($"Существует более одного дома с возможными номерами '{housesNum}'. Возвращается первый дом с номером '{fstItem?.HouseNum}' и ИД '{fstItem?.HouseId}'.");
                    return fstItem?.HouseId;
                }

                AddrObjHouse houseItem = houses.Items.FirstOrDefault();
                return houseItem?.HouseId;
            }
            catch (Exception ex)
            {
                _log.Warning(ex, ex.Message);
                return null;
            }
        }
    }
}