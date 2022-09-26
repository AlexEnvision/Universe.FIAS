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
    ///     Режимы сопоставления домов
    /// </summary>
    public enum HousesMatchMode
    {
        /// <summary>
        ///     Пытатся точно сопоставить родительский объект для дома в виде кода улицы.
        ///     Коды адреса одиночные в операциях.
        /// </summary>
        AoParentExactSingle = 1,

        /// <summary>
        ///     Пытатся точно сопоставить родительский объект для дома в виде кода улицы.
        ///     Коды адреса множественные в операциях.
        /// </summary>
        AoParentExactMultiple = 2,

        /// <summary>
        ///     Использовать для сравнения только номера домов.
        ///     Родительские объекты улиц игнорируются полностью.
        ///     Внимание! Возможно получение домов, относящихся к другим регионам и улицам
        /// </summary>
        OnlyHouseNumbers = 3
    }
}