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

namespace Universe.Fias.Normalizer.Algorithms.AddressSystem.Settings
{
    /// <summary>
    ///     Настройки нормализации адреса
    /// </summary>
    public class NormalizeAddressSettings
    {
        /// <summary>
        ///     Разрешить нормализацию домов
        /// </summary>
        public bool IncludeHouses { get; set; }

        /// <summary>
        ///     Игнорировать актуальность.
        ///     Применяется только к улицам.
        /// </summary>
        public bool ActualIgnore { get; set; }

        /// <summary>
        ///     Режим сопоставления улиц
        /// </summary>
        public StreetMatchMode StreetMatchMode { get; set; }

        /// <summary>
        ///     Режим сопоставления домов
        /// </summary>
        public HousesMatchMode HousesMatchMode { get; set; }

        /// <summary>
        ///     Расширенные паттерны поиска для региона
        ///     (Разбиение на фрагменты, включение всей строки без обрезки адресного объекта).
        /// </summary>
        public bool RegionExtendSearchPatterns { get; set; }

        /// <summary>
        ///     Расширенные паттерны поиска для районов
        ///     (Разбиение на фрагменты, включение всей строки без обрезки адресного объекта).
        /// </summary>
        public bool DistrictExtendSearchPatterns { get; set; }

        /// <summary>
        ///     Расширенные паттерны поиска для городов и населённых пунктов
        ///     (Разбиение на фрагменты, включение всей строки без обрезки адресного объекта).
        /// </summary>
        public bool SettlementExtendSearchPatterns { get; set; }

        /// <summary>
        ///     Настройки нормализации по умолчанию
        /// </summary>
        public static readonly NormalizeAddressSettings Default = new NormalizeAddressSettings {
            IncludeHouses = true,
            StreetMatchMode = StreetMatchMode.AoParentExact,
            HousesMatchMode = HousesMatchMode.AoParentExactSingle,

            RegionExtendSearchPatterns = true,
            DistrictExtendSearchPatterns = true,
            SettlementExtendSearchPatterns = true,
        };

        /// <summary>
        ///     Пресет настроек с установленнысм разрешением без учёта номеров домов.
        /// </summary>
        public static readonly NormalizeAddressSettings IgnoreHouses = new NormalizeAddressSettings
        {
            IncludeHouses = false,
            StreetMatchMode = StreetMatchMode.AoParentExact,
            HousesMatchMode = HousesMatchMode.AoParentExactSingle,

            RegionExtendSearchPatterns = true,
            DistrictExtendSearchPatterns = true,
            SettlementExtendSearchPatterns = true,
        };

        /// <summary>
        ///     Пресет настроек с установленнысм разрешением без учёта номеров домов и игнорирования актуальности в БД ФИАС.
        /// </summary>
        public static readonly NormalizeAddressSettings IgnoreActualAndHouses = new NormalizeAddressSettings
        {
            IncludeHouses = false,
            ActualIgnore = true,
            StreetMatchMode = StreetMatchMode.AoParentExact,
            HousesMatchMode = HousesMatchMode.AoParentExactSingle,

            RegionExtendSearchPatterns = true,
            DistrictExtendSearchPatterns = true,
            SettlementExtendSearchPatterns = true,
        };

        /// <summary>
        ///     Пресет настроек с установленнысм разрешением по названиям улиц, но без учёта номеров домов и без учёта родительских адресных объектов.
        ///     Внимание! Возможно получение улиц, относящихся к другим регионам и улицам
        /// </summary>
        public static readonly NormalizeAddressSettings StreetNamesMatchesButIgnoreHouses = new NormalizeAddressSettings
        {
            IncludeHouses = false,
            StreetMatchMode = StreetMatchMode.OnlyStreetNames,
            HousesMatchMode = HousesMatchMode.OnlyHouseNumbers,

            RegionExtendSearchPatterns = true,
            DistrictExtendSearchPatterns = true,
            SettlementExtendSearchPatterns = true,
        };

        /// <summary>
        ///     Пресет настроек с установленнысм разрешением по номерам домов и без учёта родительских адресных объектов.
        ///     Внимание! Возможно получение домов, относящихся к другим регионам и улицам
        /// </summary>
        public static readonly NormalizeAddressSettings HouseNumberMatches = new NormalizeAddressSettings
        {
            IncludeHouses = true,
            StreetMatchMode = StreetMatchMode.AoParentExact,
            HousesMatchMode = HousesMatchMode.OnlyHouseNumbers,

            RegionExtendSearchPatterns = true,
            DistrictExtendSearchPatterns = true,
            SettlementExtendSearchPatterns = true,
        };

        /// <summary>
        ///     Пресет настроек с установленнысм разрешением по названиям улиц и по номерам домов и без учёта родительских адресных объектов.
        ///     Внимание! Возможно получение улиц и домов, относящихся к другим регионам и улицам
        /// </summary>
        public static readonly NormalizeAddressSettings StreetNamesHouseNumberMatches = new NormalizeAddressSettings
        {
            IncludeHouses = true,
            StreetMatchMode = StreetMatchMode.OnlyStreetNames,
            HousesMatchMode = HousesMatchMode.OnlyHouseNumbers,

            RegionExtendSearchPatterns = true,
            DistrictExtendSearchPatterns = true,
            SettlementExtendSearchPatterns = true,
        };
    }
}