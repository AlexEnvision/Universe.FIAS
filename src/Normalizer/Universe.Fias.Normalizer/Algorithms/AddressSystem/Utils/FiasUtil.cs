//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2022 Universe.FIAS                                                  ║
//  ║                                                                                 ║
//  ║   Licensed under the Apache License, Version 2.0 (the "License");               ║
//  ║   you may not use this file except in compliance with the License.              ║
//  ║   You may obtain a copy of the License at                                       ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0                                ║
//  ║                                                                                 ║
//  ║   Unless required by applicable law or agreed to in writing, software           ║
//  ║   distributed under the License is distributed on an "AS IS" BASIS,             ║
//  ║   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.      ║
//  ║   See the License for the specific language governing permissions and           ║
//  ║   limitations under the License.                                                ║
//  ║                                                                                 ║
//  ║                                                                                 ║
//  ║   Copyright 2022 Universe.FIAS                                                  ║
//  ║                                                                                 ║
//  ║   Лицензировано согласно Лицензии Apache, Версия 2.0 ("Лицензия");              ║
//  ║   вы можете использовать этот файл только в соответствии с Лицензией.           ║
//  ║   Вы можете найти копию Лицензии по адресу                                      ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0.                               ║
//  ║                                                                                 ║
//  ║   За исключением случаев, когда это регламентировано существующим               ║
//  ║   законодательством или если это не оговорено в письменном соглашении,          ║
//  ║   программное обеспечение распространяемое на условиях данной Лицензии,         ║
//  ║   предоставляется "КАК ЕСТЬ" и любые явные или неявные ГАРАНТИИ ОТВЕРГАЮТСЯ.    ║
//  ║   Информацию об основных правах и ограничениях,                                 ║
//  ║   применяемых к определенному языку согласно Лицензии,                          ║
//  ║   вы можете найти в данной Лицензии.                                            ║
//  ║                                                                                 ║
//  ╚═════════════════════════════════════════════════════════════════════════════════╝

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Universe.CQRS.Dal.Queries;
using Universe.CQRS.Extensions;
using Universe.CQRS.Infrastructure;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.DataAccess.Models;
using Universe.Fias.Helpers;
using Universe.Helpers.Extensions;
using Universe.Types.Collection;

namespace Universe.Fias.Normalizer.Algorithms.AddressSystem.Utils
{
	/// <inheritdoc/>
	public class FiasUtil : IFiasUtil
	{
        private readonly List<int> _regionLevels = new List<int> { 1, 2, 3 };
        private readonly List<int> _districtLevels = new List<int> { 3, 5, 35, 90 };
        private readonly List<int> _cityLevels = new List<int> { 3, 4, 6, 7, 35, 65 };
        private readonly List<int> _locationLevels = new List<int> { 3, 4, 6, 7, 35, 65, 90 };
        private readonly List<int> _streetLevels = new List<int> { 7, 65, 91 };

        private Lazy<List<string>> _regionAddressTypes;

        private Lazy<List<string>> _districtAddressTypes;

        private readonly Lazy<List<string>> _cityAddressTypes;

        private readonly Lazy<List<string>> _localityAddressTypes;

        private readonly Lazy<List<string>> _streetAddressTypes;

        private readonly UniverseFiasScope _scope;

        private MatList<AddressTypeDto> _allAddressTypes;

        /// <summary>
		///		Инициализирует экземпляр класса <see cref="FiasUtil"/>
		/// </summary>
		public FiasUtil(UniverseFiasScope scope)
		{
			if (scope == null)
				throw new ArgumentNullException(nameof(scope));

            _regionAddressTypes = new Lazy<List<string>>(() =>
            {
                return GetAddressTypesByLevels(_regionLevels);
            });

            _districtAddressTypes = new Lazy<List<string>>(() =>
            {
                return GetAddressTypesByLevels(_districtLevels);
            });

            _cityAddressTypes = new Lazy<List<string>>(() =>
			{
				return GetAddressTypesByLevels(_cityLevels);
			});

            _localityAddressTypes = new Lazy<List<string>>(() =>
            {
                return GetAddressTypesByLevels(_locationLevels);
            });

            _streetAddressTypes = new Lazy<List<string>>(() =>
			{
				return GetAddressTypesByLevels(_streetLevels);
			});

            _scope = scope;
            _allAddressTypes = null;
        }

        private MatList<AddressTypeDto> ExternAddressTypes(MatList<AddressTypeDto> allAddressTypes)
        {
			var lastIndex = allAddressTypes.OrderByDescending(x => x.Id).FirstOrDefault()?.Id ?? 0;

            var types = new MatList<AddressTypeDto>();
            types += new AddressTypeDto { Id = ++lastIndex, Name = "Проспект", ShortName = "п-кт", Level = 65 };
			types += new AddressTypeDto { Id = ++lastIndex, Name = "Проспект", ShortName = "пр.", Level = 65 };
            types += new AddressTypeDto { Id = ++lastIndex, Name = "Проспект", ShortName = "просп", Level = 65 };

            types += new AddressTypeDto { Id = ++lastIndex, Name = "Проезд", ShortName = "пр.", Level = 65 };

            return types;
        }

        private MatList<AddressTypeDto> GetAllAddressTypes()
        {
            var records =
                _scope.GetQuery<SelectEntitiesQuery<AsAddrObjType, AddressTypeDto>>().Execute(
                    EntityReqHelper.GetAnyReq(
                        MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<AsAddrObjType>(nameof(AsAddrObjType.Id), x => x.Id)
                        ),
                        allItemsAsOnePage: true
                    ),
                    projection: item =>
                        new AddressTypeDto
                        {
                            Id = item.Id,
                            Name = item.Name,
                            ShortName = item.ShortName,
                            Level = item.Level ?? 0,
                            Title = item.Name
                        }
                ).Items.ToMatList();

            return records + ExternAddressTypes(records);
        }

		private MatList<string> GetAddressTypesByLevels(List<int> levels)
        {
            _allAddressTypes ??= GetAllAddressTypes();

			var result = new MatList<string>();
			foreach (var record in _allAddressTypes.Where(t => levels.Contains(t.Level)))
			{
				if (!result.Contains(record.ShortName))
					result.Add(record.ShortName);
				if (!result.Contains(record.Name))
					result.Add(record.Name);
			}
			return result;
		}

        /// <inheritdoc/>
        public string GetRegionWithoutType(string region)
        {
            var result = region
                .Trim()
                .TrimStart(_regionAddressTypes.Value)
                .TrimEnd(_regionAddressTypes.Value);

            return result;
        }

        /// <inheritdoc/>
        public string GetDistrictWithoutType(string district)
        {
            var result = district
                .Trim()
                .TrimStart(_districtAddressTypes.Value)
                .TrimEnd(_districtAddressTypes.Value);

            return result;
        }

        /// <inheritdoc/>
		public string GetCityWithoutType(string city)
		{
			var result = city
                .Trim()
                .TrimStart(_cityAddressTypes.Value)
                .TrimEnd(_cityAddressTypes.Value)
                .Replace("г.", "");

            return result;
		}

        /// <inheritdoc/>
        public string GetLocationWithoutType(string locality)
        {
            var result = locality
                .Trim()
                .TrimStart(_localityAddressTypes.Value)
                .TrimEnd(_localityAddressTypes.Value);

            return result;
        }

        /// <inheritdoc/>
        public string GetStreetWithoutType(string street)
        {
			var result = street
                .Trim()
                .TrimStart(_streetAddressTypes.Value)
                .TrimEnd(_streetAddressTypes.Value);

            return result;
		}

        /// <inheritdoc/>
        public string ResolveType(string type)
        {
            var typeByAddressTypeDict =
                _allAddressTypes.FirstOrDefault(x => x.ShortName.PrepareToCompare() == type.PrepareToCompare());
            if (typeByAddressTypeDict != null)
                return typeByAddressTypeDict.Name;

            return type;
        }

        /// <inheritdoc/>
        public List<string> ResolveTypes(string type)
        {
            var typeByAddressTypeDict =
                _allAddressTypes.Where(x => x.ShortName.PrepareToCompare() == type.PrepareToCompare()).ToList();
            if (typeByAddressTypeDict.Count > 0)
                return typeByAddressTypeDict.Select(x => x.Name).ToList();

            return new List<string> { type };
        }

        /// <inheritdoc/>
		public FiasInfo GetFiasInfo(string address)
		{
			var streetWoType = address.Trim().TrimStart(_streetAddressTypes.Value);
			
			var match = Regex.Match(streetWoType, @"^([^,]+),[^\d]*([\d/]+)", RegexOptions.IgnoreCase);
			
			if(match.Success)
				return new FiasInfo { Street = match.Groups[1].Value, House = match.Groups[2].Value };

			match = Regex.Match(streetWoType, @"^(\w+)[^\d]*([\d/]+)", RegexOptions.IgnoreCase);

			if (match.Success)
				return new FiasInfo { Street = match.Groups[1].Value, House = match.Groups[2].Value };

			return null;
		}
    }
}