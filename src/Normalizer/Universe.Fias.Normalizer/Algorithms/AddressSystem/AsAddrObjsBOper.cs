using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Universe.CQRS.Dal.Queries;
using Universe.CQRS.Extensions;
using Universe.CQRS.Infrastructure;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.DataAccess;
using Universe.Fias.DataAccess.Models;
using Universe.Fias.Helpers;
using Universe.Fias.Normalizer.Algorithms.AddressSystem.Dto;
using Universe.Fias.Normalizer.Models;
using Universe.Fias.Normalizer.Models.AddressSystem;
using Universe.Fias.Normalizer.Models.AddressSystem.Field;
using Universe.Helpers.Extensions;
using Universe.Types;

namespace Universe.Fias.Normalizer.Algorithms.AddressSystem
{
    /// <summary>
    ///     Address system gets AddrObjs and houses operations.
    /// </summary>
    /// <seealso cref="DisposableObject"/>
    public class AsAddrObjsBOper : DisposableObject
    {
        private UniverseFiasDbContext _db;
        private Dictionary<string, List<AsAddrObjType>> _indexedByNameTypes;
        private List<AsAddrObjType> _cityTypes;
        private UniverseFiasScope _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsAddrObjsBOper"/> class.
        /// </summary>
        /// <param name="scope">The Scope.</param>
        public AsAddrObjsBOper(UniverseFiasScope scope)
        {
            _db = scope.Factory.CreateUniverseFiasDb();
            _db.Database.CommandTimeout = 60 * 60;

            _scope = scope;

            Initialize();
        }

        private void Initialize()
        {
            var types = GetAddrTypes();
            _cityTypes = types;
            _indexedByNameTypes = types.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList());
        }

        /// <summary>
        /// Gets the length of the code.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public static int GetCodeLength(AsAoLevels level)
        {
            var codeLength = 0;
            switch (level)
            {
                case AsAoLevels.Region:
                    codeLength = 2;
                    break;
                case AsAoLevels.District:
                    codeLength = 2 + 3;
                    break;
                case AsAoLevels.City:
                    codeLength = 2 + 3 + 3;
                    break;
                case AsAoLevels.Location:
                    codeLength = 2 + 3 + 3 + 3;
                    break;
                case AsAoLevels.Level65:
                    codeLength = 2 + 3 + 3 + 3 + 4;
                    break;
                case AsAoLevels.Street:
                    codeLength = 2 + 3 + 3 + 3 + 4;
                    break;
            }

            return codeLength;
        }

        /// <summary>
        ///     Get address types
        /// </summary>
        /// <returns></returns>
        private List<AsAddrObjType> GetAddrTypes()
        {
            var types = _db.AsAddrObjTypes.ToList();
            return types;
        }

        /// <summary>
        /// Gets the addr object.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public AsAddressFieldValueAddrObj GetAddrObj([NotNull] string code)
        {
            if (string.IsNullOrEmpty(code))
                return null;

            if (code.StartsWith(AddrCodes.PrefixArbitraryAddrCode))
                return new AsAddressFieldValueAddrObj {
                    Code = code,
                    Title = code.Replace(AddrCodes.PrefixArbitraryAddrCode, string.Empty),
                    IsArbitrary = true
                };

            var q = GetActualAddrObjs().Where(_ => _.PlainCode == code);
            return GetAddrObj(q, $"Not found AddrObj by PlainCode {code}");
        }

        /// <summary>
        ///     Получает объект адреса.
        ///     Gets the addr object.
        /// </summary>
        /// <param name="aoId">The aoId.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public AsAddressFieldValueAddrObj GetAddrObjByAoId(Guid aoId)
        {
            var q = GetActualAddrObjs().Where(_ => _.AoId == aoId);
            return GetAddrObj(q, $"Not found AddrObj by AoId {aoId}");
        }

        /// <summary>
        ///     Получает объект адреса. Безопасная версия.
        ///     Gets the addr object. Safe version.
        /// </summary>
        /// <param name="aoId">The aoId.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public AsAddressFieldValueAddrObj GetAddrObjByAoIdSafe(Guid aoId)
        {
            try
            {
                return GetAddrObjByAoId(aoId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the addr objs.
        /// </summary>
        /// <param name="parentAoId">The parent ao identifier.</param>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="page">The page.</param>
        /// <param name="counOnPage">The coun on page.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        public AddrObjResponse GetAddrObjs(Guid? parentAoId, string searhTerm, int page, int counOnPage = 10)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (counOnPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(counOnPage));

            var q = GetActualAddrObjs().Where(_ => _.ParentAoId == parentAoId);

            return GetAddrObjs(q, searhTerm, page, counOnPage);
        }

        /// <summary>
        /// Gets the cities.
        /// </summary>
        /// <param name="regionCode">The region code.</param>
        /// <param name="districtCode">The district code.</param>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="page">The page.</param>
        /// <param name="counOnPage">The coun on page.</param>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        public AddrObjResponse GetCities(
            string regionCode,
            string districtCode,
            string searhTerm,
            int page,
            int counOnPage = 10,
            string fullName = null)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (counOnPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(counOnPage));

            var levels = new[] {
                (int)AsAoLevels.Region,
                (int)AsAoLevels.City,
                (int)AsAoLevels.Location
            };

            var q = GetActualAddrObjs().Where(_ => levels.Contains(_.AoLevel.Value) && _.Type.Name == "Город");

            return GetAddrObjs(
                q,
                searhTerm,
                page,
                counOnPage,
                regionCode,
                districtCode,
                joinRegions: regionCode.IsNullOrEmpty(),
                joinDistricts: districtCode.IsNullOrEmpty(),
                fullName: fullName);
        }

        /// <summary>
        /// Gets the districts.
        /// </summary>
        /// <param name="regionCode">The region code.</param>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="page">The page.</param>
        /// <param name="counOnPage">The coun on page.</param>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        public AddrObjResponse GetDistricts(string regionCode, string searhTerm, int page, int counOnPage = 10, string fullName = null)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (counOnPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(counOnPage));

            var q = GetActualAddrObjs().Where(_ => _.AoLevel == (int)AsAoLevels.District);

            return GetAddrObjs(q, searhTerm, page, counOnPage, regionCode, joinRegions: regionCode.IsNullOrEmpty(), fullName: fullName);
        }

        /// <summary>
        ///     Получает имформацию о доме
        ///     Gets the house.
        /// </summary>
        /// <param name="parentCode">
        ///     Код родительского адресного объекта
        ///     The parent code.
        /// </param>
        /// <param name="houseCode">
        ///     Код дома. В качестве него выступает HouseId (Guid).
        ///     The house code.
        /// </param>
        /// <exception cref="System.ArgumentNullException">parentCode</exception>
        /// <exception cref="System.Exception"></exception>
        public AsAddressFieldValueHouse GetHouse([NotNull] string parentCode, string houseCode)
        {
            if (houseCode.IsNullOrEmpty())
                return null;

            if (houseCode.StartsWith(AddrCodes.PrefixArbitraryAddrCode))
                return new AsAddressFieldValueHouse {
                    HouseIdOrCode = houseCode,
                    Title = houseCode.Replace(AddrCodes.PrefixArbitraryAddrCode, string.Empty),
                    IsArbitrary = true
                };

            if (parentCode == null)
                throw new ArgumentNullException(nameof(parentCode));

            if (parentCode.StartsWith(AddrCodes.PrefixArbitraryAddrCode))
                return null;

            var houseId = houseCode.ToGuidTryParse();
            if (houseId == null)
                throw new Exception($"House by the house code '{houseCode}' not found.");
            var regionCode = parentCode.Substring(0, GetCodeLength(AsAoLevels.Region));

            var house = GetActualHouses().Where(_ => _.RegionCode == regionCode && _.HouseId == houseId).Select(
                _ => new {
                    _.AoId,
                    _.HouseId,
                    EstStatusName = _.EstStatus.Name,
                    _.HouseNum,
                    _.BuildNum,
                    StrStatusName = _.StrStatus.Name,
                    _.StrucNum
                }).FirstOrDefault();
            if (house == null)
                throw new Exception($"Not found House by HouseId {houseId}");

            return new AsAddressFieldValueHouse {
                AoId = house.AoId,
                HouseIdOrCode = house.HouseId.ToString(),
                EstStatusName = house.EstStatusName,
                HouseNum = house.HouseNum,
                BuildNum = house.BuildNum,
                StrStatusName = house.StrStatusName,
                StrucNum = house.StrucNum
            };
        }

        /// <summary>
        ///     Получает имформацию о доме
        ///     Gets the house.
        /// </summary>
        /// <param name="parentCode">
        ///     Код родительского адресного объекта
        ///     The parent code.
        /// </param>
        /// <param name="houseCode">
        ///     Код дома. В качестве него выступает HouseId (Guid).
        ///     The house code.
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">parentCode</exception>
        /// <exception cref="System.Exception"></exception>
        public AsAddressFieldValueHouse GetHouseByOnlyHouseId([NotNull] string parentCode, string houseCode)
        {
            if (houseCode.IsNullOrEmpty())
                return null;

            if (houseCode.StartsWith(AddrCodes.PrefixArbitraryAddrCode))
                return new AsAddressFieldValueHouse
                {
                    HouseIdOrCode = houseCode,
                    Title = houseCode.Replace(AddrCodes.PrefixArbitraryAddrCode, string.Empty),
                    IsArbitrary = true
                };

            if (parentCode == null)
                throw new ArgumentNullException(nameof(parentCode));

            if (parentCode.StartsWith(AddrCodes.PrefixArbitraryAddrCode))
                return null;

            var houseId = houseCode.ToGuidTryParse();
            if (houseId == null)
                throw new Exception($"House by the house code '{houseCode}' not found.");
            //var regionCode = parentCode.Substring(0, GetCodeLength(AsAoLevels.Region));

            //var house = GetActualHouses()
            var house = _db.AsHouses
                .Where(_ => _.HouseId == houseId).Select(
                _ => new {
                    _.AoId,
                    _.HouseId,
                    //EstStatusName = _.EstStatus.Name,
                    _.HouseNum,
                    _.BuildNum,
                    //StrStatusName = _.StrStatus.Name,
                    _.StrucNum
                }).FirstOrDefault();
            if (house == null)
                throw new Exception($"Not found House by HouseId {houseId}");

            return new AsAddressFieldValueHouse
            {
                AoId = house.AoId,
                HouseIdOrCode = house.HouseId.ToString(),
                //EstStatusName = house.EstStatusName,
                HouseNum = house.HouseNum,
                BuildNum = house.BuildNum,
                //StrStatusName = house.StrStatusName,
                StrucNum = house.StrucNum
            };
        }

        /// <summary>
        /// Gets the houses.
        /// </summary>
        /// <param name="parentCode">The parent code.</param>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="page">The page.</param>
        /// <param name="counOnPage">The coun on page.</param>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">parentCode</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        /// <exception cref="System.Exception"></exception>
        public AddrObjHouseResponse GetHouses([NotNull] string parentCode, string searhTerm, int page, int counOnPage = 10, string fullName = null)
        {
            if (parentCode.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(parentCode));

            if (parentCode.StartsWith(AddrCodes.PrefixArbitraryAddrCode))
                return new AddrObjHouseResponse {
                    Total = 0,
                    Items = new List<AddrObjHouse>()
                };

            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (counOnPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(counOnPage));

            var parent = GetActualAddrObjs().Where(_ => _.PlainCode == parentCode)
                .Select(
                    _ => new {
                        _.AoId
                    }).FirstOrDefault();

            if (parent == null)
                throw new Exception($"AddrObj with code {parentCode} not found");

            // TODO -> Убрал проверку кода региона, т.к итак родительский элемент даёт неплохую идентификацию
            //var regionCode = parentCode.Substring(0, GetCodeLength(AsAoLevels.Region));
            var q = GetActualHouses().Where(
                _ =>
                   //_.RegionCode == regionCode && 
                     _.AoId == parent.AoId);

            searhTerm = searhTerm?.Trim();
            if (!fullName.IsNullOrEmpty())
            {
                fullName = fullName.ToLower();
                q = q.Where(_ => _.HouseNum.Equals(fullName));
            }
            else if (!searhTerm.IsNullOrEmpty())
            {
                searhTerm = searhTerm.ToLower();
                q = q.Where(
                    _ =>
                        (_.EstStatus.Name
                                + (_.HouseNum != null ? " " + _.HouseNum : string.Empty)
                                + (_.BuildNum != null ? " к. " + _.BuildNum : string.Empty)
                                + (_.StrucNum != null ? " " + _.StrStatus.Name + " " + _.StrucNum : string.Empty))
                            .ToLower().Contains(searhTerm));
            }

            var result = new AddrObjHouseResponse {
                Total = q.Count()
            };

            var query = q.Select(
                    _ => new {
                        _.HouseId,
                        EstStatusName = _.EstStatus.Name,
                        _.HouseNum,
                        _.BuildNum,
                        StrStatusName = _.StrStatus.Name,
                        _.StrucNum
                    })
                .OrderBy(_ => _.EstStatusName).ThenBy(_ => _.HouseNum).ThenBy(_ => _.BuildNum).ThenBy(_ => _.StrStatusName).ThenBy(_ => _.StrucNum)
                .Skip(counOnPage * (page - 1)).Take(counOnPage);

            result.Items = query
                .ToList()
                .Select(
                    _ => new AddrObjHouse {
                        HouseId = _.HouseId,
                        HouseNum = _.HouseNum,
                        BuildNum = _.BuildNum,
                        StrucNum = _.StrucNum,
                        EstStatusName = _.EstStatusName,
                        StrStatusName = _.StrStatusName
                    })
                .ToList();

            return result;
        }

        /// <summary>
        /// Gets the locations.
        /// </summary>
        /// <param name="regionCode">The region code.</param>
        /// <param name="districtCode">The district code.</param>
        /// <param name="cityCode">The city code.</param>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="page">The page.</param>
        /// <param name="counOnPage">The coun on page.</param>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">regionCode</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        public AddrObjResponse GetLocations(
            [NotNull] string regionCode,
            string districtCode,
            string cityCode,
            string searhTerm,
            int page,
            int counOnPage = 10,
            string fullName = null)
        {
            if (regionCode.IsNullOrEmpty() && districtCode.IsNullOrEmpty() && cityCode.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(regionCode));
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (counOnPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(counOnPage));

            var q = GetActualAddrObjs();

            var levels = new[] {
                (int)AsAoLevels.City,
                (int)AsAoLevels.Location
            };

            q = q.Where(_ => levels.Contains(_.AoLevel.Value) && _.Type.ThereAreStreets.Value && _.Type.Name != "Город");

            return GetAddrObjs(
                q,
                searhTerm,
                page,
                counOnPage,
                regionCode,
                districtCode,
                cityCode,
                regionCode.IsNullOrEmpty(),
                districtCode.IsNullOrEmpty(),
                fullName);
        }

        /// <summary>
        /// Gets the regions.
        /// </summary>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="page">The page.</param>
        /// <param name="counOnPage">The coun on page.</param>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        public AddrObjResponse GetRegions(string searhTerm, int page, int counOnPage = 10, string fullName = null)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (counOnPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(counOnPage));

            var q = GetActualAddrObjs().Where(_ => _.ParentAoId == null);

            return GetAddrObjs(q, searhTerm, page, counOnPage, fullName: fullName);
        }

        /// <summary>
        /// Gets the streets.
        /// </summary>
        /// <param name="parentCode">The parent code.</param>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="page">The page.</param>
        /// <param name="counOnPage">The coun on page.</param>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">parentCode</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        public AddrObjResponse GetStreets([NotNull] string parentCode, string searhTerm, int page, int counOnPage = 10, string fullName = null)
        {
            if (parentCode.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(parentCode));
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (counOnPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(counOnPage));

            var parent = GetActualAddrObjs().Where(_ => _.PlainCode == parentCode)
                .Select(
                    _ => new {
                        _.PlainCode,
                        _.AoLevel,
                        _.AoId
                    }).FirstOrDefault();

            if (parent == null)
                throw new Exception($"AddrObj with code {parentCode} not found");


            var codeLength = GetCodeLength(AsAoLevels.Location);
            var pCode = parentCode.Substring(0, codeLength);
            var q = GetActualAddrObjs().Where(_ => _.PlainCode.StartsWith(pCode) && _.AoLevel == (int)AsAoLevels.Street);

            return GetAddrObjs(q, searhTerm, page, counOnPage, fullName: fullName, parrentAoId: parent.AoId);
        }

        /// <summary>
        /// Get street type AsAddrObjType by string streetType
        /// </summary>
        /// <param name="streetType">The street type string</param>
        /// <returns></returns>
        public AsAddrObjType GetStreetType(string streetType)
        {
            return GetTypeByLevel(AsAoLevels.Street, streetType);
        }

        /// <summary>
        /// Get street types AsAddrObjType by string streetType
        /// </summary>
        /// <param name="streetType">The street type string</param>
        /// <returns></returns>
        public List<AsAddrObjType> GetStreetTypes(string streetType)
        {
            return GetTypesByLevel(AsAoLevels.Street, streetType);
        }

        /// <summary>
        /// Get type by Level
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="streetType">The street type string</param>
        /// <returns></returns>
        public AsAddrObjType GetTypeByLevel(AsAoLevels level, string streetType)
        {
            var q = _db.AsAddrObjTypes.Where(_ => _.Level == (int)level);
            return q.FirstOrDefault(
                _ => _.Name.ToLower().Equals(streetType) ||
                    _.ShortName.ToLower().Equals(streetType) ||
                    streetType.StartsWith(_.ShortName));
        }

        /// <summary>
        ///     Get type by Level
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="streetType">The street type string</param>
        /// <returns></returns>
        public List<AsAddrObjType> GetTypesByLevel(AsAoLevels level, string streetType)
        {
            var q = _db.AsAddrObjTypes.Where(_ => _.Level == (int)level);
            var types = q.Where(
                _ => _.Name.ToLower().Equals(streetType) ||
                     _.ShortName.ToLower().Equals(streetType) ||
                     streetType.StartsWith(_.ShortName)).ToList();

            return types;
        }

        /// <summary>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db?.Dispose();
                _db = null;
            }
        }

        private static AsAddressFieldValueAddrObj GetAddrObj(IQueryable<AsAddrObj> q, string message)
        {
            var addrObj = q.Select(
                _ => new {
                    _.AoId,
                    _.ParentAoId,
                    _.PlainCode,
                    _.TypeShortName,
                    _.Name
                }).FirstOrDefault();
            if (addrObj == null)
                throw new Exception(message);

            return new AsAddressFieldValueAddrObj {
                AoId = addrObj.AoId,
                ParentAoId = addrObj.ParentAoId,
                Code = addrObj.PlainCode,
                TypeShortName = addrObj.TypeShortName,
                Name = addrObj.Name,
                Title = $"{addrObj.Name} {addrObj.TypeShortName}"
            };
        }

        private IQueryable<AsAddrObj> GetActualAddrObjs()
        {
            var divTypes = new[] {
                (int)AsAoDivTypes.Undefined,
                (int)AsAoDivTypes.Municipal,
                (int)AsAoDivTypes.Administrative
            };
            var actual = (int)AsActStatuses.Actual;
            var q = _db.AsAddrObjs.Where(_ => _.LiveStatusId == actual && divTypes.Contains(_.DivType.Value));
            return q;
        }

        private IQueryable<AsHouse> GetActualHouses()
        {
            var curDate = DateTime.Now;
            var q = _db.AsHouses.Where(
                _ => _.StartDate <= curDate && curDate <= _.EndDate);
            return q;
        }

        private AddrObjResponse GetAddrObjs(
            IQueryable<AsAddrObj> q,
            string searhTerm,
            int page,
            int counOnPage,
            string regionCode = null,
            string districtCode = null,
            string cityCode = null,
            bool joinRegions = false,
            bool joinDistricts = false,
            string fullName = null,
            Guid? parrentAoId = null)
        {
            if (!regionCode.IsNullOrEmpty())
                regionCode = regionCode.Substring(0, GetCodeLength(AsAoLevels.Region));

            if (!cityCode.IsNullOrEmpty())
            {
                cityCode = cityCode.Substring(0, GetCodeLength(AsAoLevels.City));
                q = q.Where(_ => _.PlainCode.StartsWith(cityCode));
            }
            else if (!districtCode.IsNullOrEmpty())
            {
                districtCode = districtCode.Substring(0, GetCodeLength(AsAoLevels.District));
                q = q.Where(_ => _.PlainCode.StartsWith(districtCode));
            }
            else if (!regionCode.IsNullOrEmpty())
            {
                q = q.Where(_ => _.PlainCode.StartsWith(regionCode));
            }

            searhTerm = searhTerm?.Trim();
            if (!fullName.IsNullOrEmpty())
            {
                fullName = fullName.ToLower();
                q = q.Where(_ => _.Name.ToLower().Equals(fullName));
            }
            else if (!searhTerm.IsNullOrEmpty())
            {
                searhTerm = searhTerm.ToLower();
                var searhTermContains = " " + searhTerm;
                var searhTermContains1 = "." + searhTerm;
                var searhTermContains2 = "-" + searhTerm;
                q =
                    q.Where(
                        _ =>
                            (_.Name + " " + _.TypeShortName).ToLower().StartsWith(searhTerm) ||
                            (_.Name + " " + _.TypeShortName).ToLower().Contains(searhTermContains) ||
                            (_.Name + " " + _.TypeShortName).ToLower().Contains(searhTermContains1) ||
                            (_.Name + " " + _.TypeShortName).ToLower().Contains(searhTermContains2));
            }

            var result = new AddrObjResponse {
                Total = q.Count()
            };

            var regionCodeLength = GetCodeLength(AsAoLevels.Region);
            var districtCodeLength = GetCodeLength(AsAoLevels.District);

            //if (joinRegions && joinDistricts)
            //{
            //    var query = from ao in q
            //        join r in GetActualAddrObjs().Where(_ => _.ParentAoId == null) // joinRegions &&
            //        on ao.PlainCode.Substring(0, regionCodeLength) equals r.PlainCode.Substring(0, regionCodeLength)
            //        into r1
            //        from reg in r1.DefaultIfEmpty()
            //        join d in GetActualAddrObjs().Where(_ => _.AoLevel == (int)AsAoLevels.District) // joinDistricts && 
            //        on ao.PlainCode.Substring(0, districtCodeLength) equals d.PlainCode.Substring(0, districtCodeLength)
            //        into d1
            //        from dis in d1.DefaultIfEmpty()
            //        select new {
            //            ao.AoId,
            //            ao.PlainCode,
            //            ao.TypeShortName,
            //            ao.Name,
            //            RegTypeShortName = reg.TypeShortName,
            //            RegName = reg.Name,
            //            RegCode = reg.PlainCode,
            //            DistrTypeShortName = dis.TypeShortName,
            //            DistrName = dis.Name,
            //            DistrCode = dis.PlainCode
            //        };
            //    query = query.OrderBy(_ => _.Name).ThenBy(_ => _.RegName).ThenBy(_ => _.DistrName)
            //        .Skip(counOnPage * (page - 1)).Take(counOnPage);

            //    result.Items = query
            //        .ToList()
            //        .Select(
            //            _ => new AddrObj {
            //                AoId = _.AoId,
            //                Code = _.PlainCode,
            //                TypeShortName = _.TypeShortName,
            //                Name = _.Name,
            //                Region = !_.RegCode.IsNullOrEmpty()
            //                    ? new AddrObj {
            //                        Code = _.RegCode,
            //                        TypeShortName = _.RegTypeShortName,
            //                        Name = _.RegName
            //                    }
            //                    : null,
            //                District = !_.DistrCode.IsNullOrEmpty()
            //                    ? new AddrObj {
            //                        Code = _.DistrCode,
            //                        TypeShortName = _.DistrTypeShortName,
            //                        Name = _.DistrName
            //                    }
            //                    : null
            //            })
            //        .ToList();

            //    return result;
            //}

            //if (joinRegions)
            //{
            //    var query = from ao in q
            //        join r in GetActualAddrObjs().Where(_ => _.ParentAoId == null) // joinRegions &&
            //        on ao.PlainCode.Substring(0, regionCodeLength) equals r.PlainCode.Substring(0, regionCodeLength)
            //        into r1
            //        from reg in r1.DefaultIfEmpty()
            //        select new {
            //            ao.AoId,
            //            ao.PlainCode,
            //            ao.TypeShortName,
            //            ao.Name,
            //            RegTypeShortName = reg.TypeShortName,
            //            RegName = reg.Name,
            //            RegCode = reg.PlainCode
            //        };
            //    query = query.OrderBy(_ => _.Name).ThenBy(_ => _.RegName)
            //        .Skip(counOnPage * (page - 1)).Take(counOnPage);

            //    result.Items = query
            //        .ToList()
            //        .Select(
            //            _ => new AddrObj {
            //                AoId = _.AoId,
            //                Code = _.PlainCode,
            //                TypeShortName = _.TypeShortName,
            //                Name = _.Name,
            //                Region = !_.RegCode.IsNullOrEmpty()
            //                    ? new AddrObj {
            //                        Code = _.RegCode,
            //                        TypeShortName = _.RegTypeShortName,
            //                        Name = _.RegName
            //                    }
            //                    : null
            //            })
            //        .ToList();

            //    return result;
            //}

            //if (joinDistricts)
            //{
            //    var query = from ao in q
            //        join d in GetActualAddrObjs().Where(_ => _.AoLevel == (int)AsAoLevels.District && _.PlainCode.StartsWith(regionCode))
            //        // joinDistricts && 
            //        on ao.PlainCode.Substring(0, districtCodeLength) equals d.PlainCode.Substring(0, districtCodeLength)
            //        into d1
            //        from dis in d1.DefaultIfEmpty()
            //        select new {
            //            ao.AoId,
            //            ao.PlainCode,
            //            ao.TypeShortName,
            //            ao.Name,
            //            DistrTypeShortName = dis.TypeShortName,
            //            DistrName = dis.Name,
            //            DistrCode = dis.PlainCode
            //        };
            //    query = query.OrderBy(_ => _.Name).ThenBy(_ => _.DistrName)
            //        .Skip(counOnPage * (page - 1)).Take(counOnPage);

            //    result.Items = query
            //        .ToList()
            //        .Select(
            //            _ => new AddrObj {
            //                AoId = _.AoId,
            //                Code = _.PlainCode,
            //                TypeShortName = _.TypeShortName,
            //                Name = _.Name,
            //                District = !_.DistrCode.IsNullOrEmpty()
            //                    ? new AddrObj {
            //                        Code = _.DistrCode,
            //                        TypeShortName = _.DistrTypeShortName,
            //                        Name = _.DistrName
            //                    }
            //                    : null
            //            })
            //        .ToList();

            //    return result;
            //}

            {
                var query = from ao in q
                    select new {
                        ao.AoId,
                        ao.ParentAoId,
                        ao.PlainCode,
                        ao.TypeShortName,
                        ao.Name
                    };
                query = query.OrderBy(_ => _.Name).ThenBy(_ => _.PlainCode)
                    .Skip(counOnPage * (page - 1)).Take(counOnPage);

                result.Items = query
                    .ToList()
                    .Select(
                        _ => new AddrObj {
                            AoId = _.AoId,
                            ParentAoId = _.ParentAoId,
                            Code = _.PlainCode,
                            TypeShortName = _.TypeShortName,
                            Name = _.Name
                        })
                    .ToList();

                if (joinRegions)
                {
                    var regionCodes =
                        result.Items.Where(_ => !_.Code.IsNullOrEmpty()).Select(_ => _.Code.Substring(0, regionCodeLength)).Distinct().ToList();

                    var regionsDic = GetActualAddrObjs()
                        .Where(_ => _.AoLevel == (int)AsAoLevels.Region && regionCodes.Contains(_.AoCode))
                        .Select(
                            _ => new {
                                _.AoId,
                                _.PlainCode,
                                _.TypeShortName,
                                _.Name,
                                _.AoCode
                            }).ToDictionary(
                            _ => _.AoCode,
                            _ => new AddrObj {
                                AoId = _.AoId,
                                Code = _.PlainCode,
                                TypeShortName = _.TypeShortName,
                                Name = _.Name
                            });

                    result.Items.ForEach(
                        _ => {
                            AddrObj region;
                            regionsDic.TryGetValue(_.Code.Substring(0, regionCodeLength), out region);
                            _.Region = region;
                        });
                }

                if (joinDistricts)
                {
                    var districtCodes =
                        result.Items.Where(_ => !_.Code.IsNullOrEmpty()).Select(_ => _.Code.Substring(0, districtCodeLength)).Distinct().ToList();

                    var districtsDic = GetActualAddrObjs()
                        .Where(_ => _.AoLevel == (int)AsAoLevels.District && districtCodes.Contains(_.AoCode))
                        .Select(
                            _ => new {
                                _.AoId,
                                _.PlainCode,
                                _.TypeShortName,
                                _.Name,
                                _.AoCode
                            }).ToDictionary(
                            _ => _.AoCode,
                            _ => new AddrObj {
                                AoId = _.AoId,
                                Code = _.PlainCode,
                                TypeShortName = _.TypeShortName,
                                Name = _.Name
                            });

                    result.Items.ForEach(
                        _ => {
                            AddrObj district;
                            districtsDic.TryGetValue(_.Code.Substring(0, districtCodeLength), out district);
                            _.District = district;
                        });
                }

                if (parrentAoId != null)
                {
                    var parentAoIds =
                        result.Items.Where(_ => _.ParentAoId != null && _.ParentAoId != parrentAoId)
                            .Select(_ => _.ParentAoId.Value)
                            .Distinct()
                            .ToList();

                    var parentList = GetActualAddrObjs()
                        .Where(_ => parentAoIds.Contains(_.AoId))
                        .Select(
                            _ => new {
                                _.AoId,
                                _.PlainCode,
                                _.TypeShortName,
                                _.Name,
                                _.AoCode,
                                _.UpdateDate
                            })
                        .ToList();

                    var parentDic = new Dictionary<Guid, AddrObj>();
                    foreach (var v in parentList.GroupBy(_ => _.AoId))
                    {
                        var ao = v.OrderByDescending(_ => _.UpdateDate).First();
                        parentDic.Add(
                            v.Key,
                            new AddrObj {
                                AoId = ao.AoId,
                                Code = ao.PlainCode,
                                TypeShortName = ao.TypeShortName,
                                Name = ao.Name
                            });
                    }

                    result.Items.ForEach(
                        _ => {
                            if (_.ParentAoId == null)
                                return;

                            AddrObj parent;
                            parentDic.TryGetValue(_.ParentAoId.Value, out parent);
                            _.Parent = parent;
                        });
                }

                return result;
            }
        }

        private AddrObjResponse GetAddrObjsMultiple(
            IQueryable<AsAddrObj> q,
            string searhTerm,
            int page,
            int counOnPage,
            string regionCode = null,
            string districtCode = null,
            string cityCode = null,
            bool joinRegions = false,
            bool joinDistricts = false,
            bool onlyOnePage = false,
            Guid? parrentAoId = null,
            params string[] fullNames)
        {
            if (!regionCode.IsNullOrEmpty())
                regionCode = regionCode.Substring(0, GetCodeLength(AsAoLevels.Region));

            if (!cityCode.IsNullOrEmpty())
            {
                cityCode = cityCode.Substring(0, GetCodeLength(AsAoLevels.City));
                q = q.Where(_ => _.PlainCode.StartsWith(cityCode));
            }
            else if (!districtCode.IsNullOrEmpty())
            {
                districtCode = districtCode.Substring(0, GetCodeLength(AsAoLevels.District));
                q = q.Where(_ => _.PlainCode.StartsWith(districtCode));
            }
            else if (!regionCode.IsNullOrEmpty())
            {
                q = q.Where(_ => _.PlainCode.StartsWith(regionCode));
            }

            searhTerm = searhTerm?.Trim();
            if (fullNames.Length > 0)
            {
                for (var index = 0; index < fullNames.Length; index++)
                {
                    var fullName = fullNames[index];
                    fullName = fullName.PrepareToCompare();
                    fullNames[index] = fullName;
                }
                q = q.Where(_ => fullNames.Any(fullName => _.Name.Equals(fullName)));
            }
            else if (!searhTerm.IsNullOrEmpty())
            {
                searhTerm = searhTerm.ToLower();
                var searhTermContains = " " + searhTerm;
                var searhTermContains1 = "." + searhTerm;
                var searhTermContains2 = "-" + searhTerm;
                q =
                    q.Where(
                        _ =>
                            (_.Name + " " + _.TypeShortName).ToLower().StartsWith(searhTerm) ||
                            (_.Name + " " + _.TypeShortName).ToLower().Contains(searhTermContains) ||
                            (_.Name + " " + _.TypeShortName).ToLower().Contains(searhTermContains1) ||
                            (_.Name + " " + _.TypeShortName).ToLower().Contains(searhTermContains2));
            }

            //var countQ = q.AsQueryable();
            var result = new AddrObjResponse {
                Total = !onlyOnePage ? q.Count() : 0 //countQ.Select(x => x.AoId).Count()
            };

            var regionCodeLength = GetCodeLength(AsAoLevels.Region);
            var districtCodeLength = GetCodeLength(AsAoLevels.District);

            var query = from ao in q
                select new
                {
                    ao.AoId,
                    ao.ParentAoId,
                    ao.PlainCode,
                    ao.TypeShortName,
                    ao.Name
                };
            query = query.OrderBy(_ => _.Name).ThenBy(_ => _.PlainCode)
                .Skip(counOnPage * (page - 1)).Take(counOnPage);

            result.Items = query
                .ToList()
                .Select(
                    _ => new AddrObj
                    {
                        AoId = _.AoId,
                        ParentAoId = _.ParentAoId,
                        Code = _.PlainCode,
                        TypeShortName = _.TypeShortName,
                        Name = _.Name
                    })
                .ToList();

            result.Total = onlyOnePage ? result.Items.Count : result.Total;

            if (joinRegions)
            {
                var regionCodes =
                    result.Items.Where(_ => !_.Code.IsNullOrEmpty()).Select(_ => _.Code.Substring(0, regionCodeLength))
                        .Distinct().ToList();

                var regionsDict = GetActualAddrObjs()
                    .Where(_ => _.AoLevel == (int) AsAoLevels.Region && regionCodes.Contains(_.AoCode))
                    .Select(
                    _ => new
                    {
                        _.AoId,
                        _.PlainCode,
                        _.TypeShortName,
                        _.Name,
                        _.AoCode
                    }).ToDictionary(
                    _ => _.AoCode,
                    _ => new AddrObj
                    {
                        AoId = _.AoId,
                        Code = _.PlainCode,
                        TypeShortName = _.TypeShortName,
                        Name = _.Name
                    });

                result.Items.ForEach(
                    _ =>
                    {
                        AddrObj region;
                        regionsDict.TryGetValue(_.Code.Substring(0, regionCodeLength), out region);
                        _.Region = region;
                    });
            }

            if (joinDistricts)
            {
                var districtCodes =
                    result.Items.Where(_ => !_.Code.IsNullOrEmpty())
                        .Select(_ => _.Code.Substring(0, districtCodeLength)).Distinct().ToList();

                var districtsDict = GetActualAddrObjs()
                    .Where(_ => _.AoLevel == (int) AsAoLevels.District && districtCodes.Contains(_.AoCode))
                    .Select(
                        _ => new
                        {
                            _.AoId,
                            _.PlainCode,
                            _.TypeShortName,
                            _.Name,
                            _.AoCode
                        }).ToDictionary(
                    _ => _.AoCode,
                    _ => new AddrObj
                    {
                        AoId = _.AoId,
                        Code = _.PlainCode,
                        TypeShortName = _.TypeShortName,
                        Name = _.Name
                    });

                result.Items.ForEach(
                    _ =>
                    {
                        AddrObj district;
                        districtsDict.TryGetValue(_.Code.Substring(0, districtCodeLength), out district);
                        _.District = district;
                    });
            }

            if (parrentAoId != null)
            {
                var parentAoIds =
                    result.Items.Where(_ => _.ParentAoId != null && _.ParentAoId != parrentAoId)
                        .Select(_ => _.ParentAoId.Value)
                        .Distinct()
                        .ToList();

                var parentList = GetActualAddrObjs()
                    .Where(_ => parentAoIds.Contains(_.AoId))
                    .Select(
                        _ => new
                        {
                            _.AoId,
                            _.PlainCode,
                            _.TypeShortName,
                            _.Name,
                            _.AoCode,
                            _.UpdateDate
                        })
                    .ToList();

                var parentDic = new Dictionary<Guid, AddrObj>();
                foreach (var v in parentList.GroupBy(_ => _.AoId))
                {
                    var ao = v.OrderByDescending(_ => _.UpdateDate).First();
                    parentDic.Add(
                        v.Key,
                        new AddrObj
                        {
                            AoId = ao.AoId,
                            Code = ao.PlainCode,
                            TypeShortName = ao.TypeShortName,
                            Name = ao.Name
                        });
                }

                result.Items.ForEach(
                    _ =>
                    {
                        if (_.ParentAoId == null)
                            return;

                        AddrObj parent;
                        parentDic.TryGetValue(_.ParentAoId.Value, out parent);
                        _.Parent = parent;
                    });
            }

            return result;
        }

        /// <summary>
        /// Gets the regions.
        /// </summary>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="page">The page.</param>
        /// <param name="counOnPage">The coun on page.</param>
        /// <param name="fullNames">The full names.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        public AddrObjResponse GetRegionsMultiple(string searhTerm, int page, int counOnPage = 10, params string[] fullNames)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (counOnPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(counOnPage));

            var q = GetActualAddrObjs().Where(_ => _.ParentAoId == null);

            return GetAddrObjsMultiple(q, searhTerm, page, counOnPage, onlyOnePage: true, fullNames: fullNames);
        }

        /// <summary>
        /// Gets the districts.
        /// </summary>
        /// <param name="regionCode">The region code.</param>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="page">The page.</param>
        /// <param name="counOnPage">The coun on page.</param>
        /// <param name="fullNames">The full names.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        public AddrObjResponse GetDistrictsMultiple(string regionCode, string searhTerm, int page, int counOnPage = 10, params string[] fullNames)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (counOnPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(counOnPage));

            var q = GetActualAddrObjs().Where(_ => _.AoLevel == (int)AsAoLevels.District);

            return GetAddrObjsMultiple(q, searhTerm, page, counOnPage, regionCode, joinRegions: regionCode.IsNullOrEmpty(), onlyOnePage: true, fullNames: fullNames);
        }

        /// <summary>
        /// Gets the cities.
        /// </summary>
        /// <param name="regionCode">The region code.</param>
        /// <param name="districtCode">The district code.</param>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="page">The page.</param>
        /// <param name="counOnPage">The coun on page.</param>
        /// <param name="fullNames">The full names.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        public AddrObjResponse GetCitiesMultiple(
            string regionCode,
            string districtCode,
            string searhTerm,
            int page,
            int counOnPage = 10,
            params string[] fullNames)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (counOnPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(counOnPage));

            var levels = new[] {
                (int)AsAoLevels.Region,
                (int)AsAoLevels.City,
                (int)AsAoLevels.Location
            };

            _indexedByNameTypes.TryGetValue("Город", out var cityTypes);
            var cityTypesIds = cityTypes?.Select(x => x.Id).ToList() ?? new List<long>();

            var q = GetActualAddrObjs().Where(_ => levels.Contains(_.AoLevel.Value) && cityTypesIds.Contains(_.TypeId ?? 0));

            return GetAddrObjsMultiple(
                q, searhTerm, page, counOnPage, regionCode, districtCode, 
                joinRegions: regionCode.IsNullOrEmpty(), joinDistricts: districtCode.IsNullOrEmpty(), 
                onlyOnePage: true, fullNames: fullNames);
        }

        /// <summary>
        ///     Gets the locations.
        /// </summary>
        /// <param name="regionCode">The region code.</param>
        /// <param name="districtCode">The district code.</param>
        /// <param name="cityCode">The city code.</param>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="page">The page.</param>
        /// <param name="counOnPage">The coun on page.</param>
        /// <param name="fullNames">The full names.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">regionCode</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        public AddrObjResponse GetLocationsMultiple(
            [NotNull] string regionCode,
            string districtCode,
            string cityCode,
            string searhTerm,
            int page,
            int counOnPage = 10,
            params string[] fullNames)
        {
            if (regionCode.IsNullOrEmpty() && districtCode.IsNullOrEmpty() && cityCode.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(regionCode));
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (counOnPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(counOnPage));

            var q = GetActualAddrObjs();

            var levels = new[] {
                (int)AsAoLevels.City,
                (int)AsAoLevels.Location
            };

            _indexedByNameTypes.TryGetValue("Город", out var cityTypes);
            var cityTypesIds = cityTypes?.Select(x => x.Id).ToList() ?? new List<long>();
            var nonCityTypesIds = _cityTypes.Where(x => cityTypesIds.All(y => y != x.Id)).Select(x => x.Id).ToList();

            q = q.Where(_ => levels.Contains(_.AoLevel.Value) && _.Type.ThereAreStreets.Value && nonCityTypesIds.Contains(_.TypeId ?? 0));

            return GetAddrObjsMultiple(
                q,
                searhTerm,
                page,
                counOnPage,
                regionCode,
                districtCode,
                cityCode,
                regionCode.IsNullOrEmpty(),
                districtCode.IsNullOrEmpty(),
                true,
                null,
                fullNames);
        }

        /// <summary>
        ///     Gets the streets by multiple patterns.
        /// </summary>
        /// <param name="parentCode">The parent code.</param>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="page">The page.</param>
        /// <param name="actualIgnore">Actual ignore flag.</param>
        /// <param name="counOnPage">The count on page.</param>
        /// <param name="fullNames">The full names array.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">parentCode</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        public AddrObjResponse GetStreetsMultiple([NotNull] string parentCode, string searhTerm, int page, int counOnPage = 10, bool actualIgnore = false, params string[] fullNames)
        {
            if (parentCode.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(parentCode));
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (counOnPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(counOnPage));

            var parent = GetActualAddrObjs().Where(_ => _.PlainCode == parentCode)
                .Select(
                    _ => new {
                        _.PlainCode,
                        _.AoLevel,
                        _.AoId
                    }).FirstOrDefault();

            if (parent == null)
                throw new Exception($"AddrObj with code {parentCode} not found");


            var codeLength = GetCodeLength(AsAoLevels.Location);
            var pCode = parentCode.Substring(0, codeLength);

            // TODO -> В будущем перестроить _.PlainCode.StartsWith(pCode) на механизм ключевых слов
            var q = actualIgnore 
                ? _db.AsAddrObjs.Where(_ => _.PlainCode.StartsWith(pCode) && _.AoLevel == (int)AsAoLevels.Street) 
                : GetActualAddrObjs().Where(_ => _.PlainCode.StartsWith(pCode) && _.AoLevel == (int)AsAoLevels.Street);

            return GetAddrObjsMultiple(q, searhTerm, page, counOnPage, parrentAoId: parent.AoId, onlyOnePage: true, fullNames: fullNames);
        }

        /// <summary>
        ///     Получение улицы без учёта родительского адресного элемента будь то город, либо населённый пункт
        ///     Gets the streets by multiple patterns and by street names without parent address objects.
        /// </summary>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="page">The page.</param>
        /// <param name="counOnPage">The coun on page.</param>
        /// <param name="fullNames">The full names array.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">parentCode</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        public AddrObjResponse GetStreetsByOnlyStreetNamesMultiple(string searhTerm, int page, int counOnPage = 10, params string[] fullNames)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (counOnPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(counOnPage));

            var q = GetActualAddrObjs().Where(_ =>  _.AoLevel == (int)AsAoLevels.Street);
            return GetAddrObjsMultiple(q, searhTerm, page, counOnPage, onlyOnePage: true, fullNames: fullNames);
        }

        /// <summary>
        ///     Get street types AsAddrObjType by string streetType
        /// </summary>
        /// <param name="streetTypes">The street types string</param>
        /// <returns></returns>
        public List<AsAddrObjType> GetStreetTypesMultiple(params string[] streetTypes)
        {
            return GetTypesByLevelMultiple(AsAoLevels.Street, streetTypes);
        }

        /// <summary>
        /// Get type by Level
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="streetTypes">The street types string</param>
        /// <returns></returns>
        public List<AsAddrObjType> GetTypesByLevelMultiple(AsAoLevels level, params string[] streetTypes)
        {
            var q = _db.AsAddrObjTypes.Where(x => x.Level == (int)level);
            var types = q.Where(
                x => streetTypes.Any( streetType => x.Name.ToLower().Equals(streetType) ||
                     x.ShortName.ToLower().Equals(streetType) ||
                     streetType.StartsWith(x.ShortName)
                )).ToList();

            return types;
        }

        /// <summary>
        ///     Получает дома через несколько шаблонов поиска. И через несколько кодов адреса.
        ///     Gets the houses by multiple patterns.
        /// </summary>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="onlyOnePage">Only one page. Только одна страница.</param>
        /// <param name="fullNames">The full names.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">parentCode</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        /// <exception cref="System.Exception"></exception>
        public AddrObjHouseResponse GetHousesByOnlyHouseNumberMultiple(
            string searhTerm,
            bool onlyOnePage = false,
            params string[] fullNames)
        {
            int page = 1;
            int countOnPage = 3;

            var q = _db.AsHouses.AsQueryable();

            searhTerm = searhTerm?.Trim();
            if (fullNames.Length > 0)
            {
                for (var index = 0; index < fullNames.Length; index++)
                {
                    var fullName = fullNames[index];
                    fullName = fullName.PrepareToCompare();
                    fullNames[index] = fullName;
                }
                q = q.Where(_ => fullNames.Any(fullName => _.HouseNum.Equals(fullName)));
            }
            else if (!searhTerm.IsNullOrEmpty())
            {
                searhTerm = searhTerm.ToLower();
                q = q.Where(
                    _ =>
                        (_.EstStatus.Name
                                + (_.HouseNum != null ? " " + _.HouseNum : string.Empty)
                                + (_.BuildNum != null ? " к. " + _.BuildNum : string.Empty)
                                + (_.StrucNum != null ? " " + _.StrStatus.Name + " " + _.StrucNum : string.Empty))
                            .ToLower().Contains(searhTerm));
            }

            var result = new AddrObjHouseResponse
            {
                Total = !onlyOnePage ? q.Count() : 0
            };

            var query = q.Select(
                    _ => new
                    {
                        _.HouseId,
                        //EstStatusName = _.EstStatus.Name,
                        _.HouseNum,
                        _.BuildNum,
                        //StrStatusName = _.StrStatus.Name,
                        _.StrucNum
                    })
                //.OrderBy(_ => _.EstStatusName).ThenBy(_ => _.HouseNum).ThenBy(_ => _.BuildNum).ThenBy(_ => _.StrStatusName).ThenBy(_ => _.StrucNum)
                .OrderBy(_ => _.HouseNum)
                .Skip(countOnPage * (page - 1)).Take(countOnPage);

            result.Items = query
                .ToList()
                .Select(
                    _ => new AddrObjHouse
                    {
                        HouseId = _.HouseId,
                        HouseNum = _.HouseNum,
                        BuildNum = _.BuildNum,
                        StrucNum = _.StrucNum,
                        //EstStatusName = _.EstStatusName,
                        //StrStatusName = _.StrStatusName
                    })
                .ToList();

            result.Total = onlyOnePage ? result.Items.Count : result.Total;

            return result;
        }

        /// <summary>
        ///     Получает дома через несколько шаблонов поиска. И через несколько кодов адреса.
        ///     Gets the houses by multiple patterns.
        /// </summary>
        /// <param name="parentCode">The parent code.</param>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="onlyOnePage">Only one page. Только одна страница.</param>
        /// <param name="fullNames">The full names.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">parentCode</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        /// <exception cref="System.Exception"></exception>
        public AddrObjHouseResponse GetHousesMultiple(
            [NotNull] string parentCode, 
            string searhTerm,
            bool onlyOnePage = false,
            params string[] fullNames)
        {
            if (parentCode.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(parentCode));

            if (parentCode.StartsWith(AddrCodes.PrefixArbitraryAddrCode))
                return new AddrObjHouseResponse
                {
                    Total = 0,
                    Items = new List<AddrObjHouse>()
                };

            int page = 1;
            int counOnPage = 10;

            var anonParents = GetActualAddrObjs().Where(_ => _.PlainCode == parentCode)
                .Select(
                    _ => new {
                        _.AoId
                    }).ToList();

            if (anonParents.Count == 0)
                throw new Exception($"AddrObj with code {parentCode} not found");

            var parents = anonParents.Select(x =>  x.AoId).ToList();

            // TODO -> Убрал проверку кода региона, т.к итак родительский элемент даёт неплохую идентификацию
            // А так убрал зависимость от текущей даты
            //var regionCode = parentCode.Substring(0, GetCodeLength(AsAoLevels.Region));
            //var q = GetActualHouses().Where(
            //    _ =>
            //         _.RegionCode == regionCode && 
            //         _.AoId == parent.AoId);
            var q = _db.AsHouses.Where(
                _ =>
                     parents.Any(parent => _.AoId == parent));

            searhTerm = searhTerm?.Trim();
            if (fullNames.Length > 0)
            {
                for (var index = 0; index < fullNames.Length; index++)
                {
                    var fullName = fullNames[index];
                    fullName = fullName.PrepareToCompare();
                    fullNames[index] = fullName;
                }
                q = q.Where(_ => fullNames.Any(fullName => _.HouseNum.Equals(fullName)));
            }
            else if (!searhTerm.IsNullOrEmpty())
            {
                searhTerm = searhTerm.ToLower();
                q = q.Where(
                    _ =>
                        (_.EstStatus.Name
                                + (_.HouseNum != null ? " " + _.HouseNum : string.Empty)
                                + (_.BuildNum != null ? " к. " + _.BuildNum : string.Empty)
                                + (_.StrucNum != null ? " " + _.StrStatus.Name + " " + _.StrucNum : string.Empty))
                            .ToLower().Contains(searhTerm));
            }

            var result = new AddrObjHouseResponse {
                Total = !onlyOnePage ? q.Count() : 0
            };

            var query = q.Select(
                    _ => new {
                        _.HouseId,
                        EstStatusName = _.EstStatus.Name,
                        _.HouseNum,
                        _.BuildNum,
                        StrStatusName = _.StrStatus.Name,
                        _.StrucNum
                    })
                .OrderBy(_ => _.EstStatusName).ThenBy(_ => _.HouseNum).ThenBy(_ => _.BuildNum).ThenBy(_ => _.StrStatusName).ThenBy(_ => _.StrucNum)
                //.OrderBy(_ => _.HouseNum)
                .Skip(counOnPage * (page - 1)).Take(counOnPage);

            result.Items = query
                .ToList()
                .Select(
                    _ => new AddrObjHouse
                    {
                        HouseId = _.HouseId,
                        HouseNum = _.HouseNum,
                        BuildNum = _.BuildNum,
                        StrucNum = _.StrucNum,
                        EstStatusName = _.EstStatusName,
                        StrStatusName = _.StrStatusName
                    })
                .ToList();
            result.Total = onlyOnePage ? result.Items.Count : result.Total;

            return result;
        }

        /// <summary>
        ///     Получает дома через несколько шаблонов поиска. И через несколько кодов адреса.
        ///     Gets the houses by multiple patterns.
        /// </summary>
        /// <param name="parentCodes">The parent codes.</param>
        /// <param name="searhTerm">The searh term.</param>
        /// <param name="onlyOnePage">Only one page. Только одна страница.</param>
        /// <param name="fullNames">The full names.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">parentCode</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// page
        /// or
        /// counOnPage
        /// </exception>
        /// <exception cref="System.Exception"></exception>
        public AddrObjHouseResponse GetHousesMultiple(
            [NotNull] string[] parentCodes,
            string searhTerm,
            bool onlyOnePage = false,
            params string[] fullNames)
        {
            if (parentCodes == null)
                throw new ArgumentNullException(nameof(parentCodes));

            if (parentCodes.Length == 0)
                throw new ArgumentException(nameof(parentCodes));

            if (parentCodes.All(parentCode => parentCode.StartsWith(AddrCodes.PrefixArbitraryAddrCode)))
                return new AddrObjHouseResponse
                {
                    Total = 0,
                    Items = new List<AddrObjHouse>()
                };

            int page = 1;
            int counOnPage = 10;

            var anonParents = GetActualAddrObjs().Where(_ => parentCodes.Any(parentCode => _.PlainCode == parentCode))
                .Select(
                    _ => new {
                        _.AoId
                    }).ToList();

            if (anonParents.Count == 0)
            {
                var codes = string.Join(", ", parentCodes);
                throw new Exception($"AddrObj with code {codes} not found");
            }

            var parents = anonParents.Select(x => x.AoId).ToList();

            // TODO -> Убрал проверку кода региона, т.к итак родительский элемент даёт неплохую идентификацию
            // А так убрал зависимость от текущей даты
            //var regionCode = parentCode.Substring(0, GetCodeLength(AsAoLevels.Region));
            //var q = GetActualHouses().Where(
            //    _ =>
            //         _.RegionCode == regionCode && 
            //         _.AoId == parent.AoId);
            var q = _db.AsHouses.Where(
                _ =>
                     parents.Any(parent => _.AoId == parent));

            searhTerm = searhTerm?.Trim();
            if (fullNames.Length > 0)
            {
                for (var index = 0; index < fullNames.Length; index++)
                {
                    var fullName = fullNames[index];
                    fullName = fullName.PrepareToCompare();
                    fullNames[index] = fullName;
                }
                q = q.Where(_ => fullNames.Any(fullName => _.HouseNum.Equals(fullName)));
            }
            else if (!searhTerm.IsNullOrEmpty())
            {
                searhTerm = searhTerm.ToLower();
                q = q.Where(
                    _ =>
                        (_.EstStatus.Name
                                + (_.HouseNum != null ? " " + _.HouseNum : string.Empty)
                                + (_.BuildNum != null ? " к. " + _.BuildNum : string.Empty)
                                + (_.StrucNum != null ? " " + _.StrStatus.Name + " " + _.StrucNum : string.Empty))
                            .ToLower().Contains(searhTerm));
            }

            var result = new AddrObjHouseResponse
            {
                Total = !onlyOnePage ? q.Count() : 0
            };

            var query = q.Select(
                    _ => new {
                        _.HouseId,
                        EstStatusName = _.EstStatus.Name,
                        _.HouseNum,
                        _.BuildNum,
                        StrStatusName = _.StrStatus.Name,
                        _.StrucNum
                    })
                .OrderBy(_ => _.EstStatusName).ThenBy(_ => _.HouseNum).ThenBy(_ => _.BuildNum).ThenBy(_ => _.StrStatusName).ThenBy(_ => _.StrucNum)
                //.OrderBy(_ => _.HouseNum)
                .Skip(counOnPage * (page - 1)).Take(counOnPage);

            result.Items = query
                .ToList()
                .Select(
                    _ => new AddrObjHouse
                    {
                        HouseId = _.HouseId,
                        HouseNum = _.HouseNum,
                        BuildNum = _.BuildNum,
                        StrucNum = _.StrucNum,
                        EstStatusName = _.EstStatusName,
                        StrStatusName = _.StrStatusName
                    })
                .ToList();
            result.Total = onlyOnePage ? result.Items.Count : result.Total;

            return result;
        }
    }
}