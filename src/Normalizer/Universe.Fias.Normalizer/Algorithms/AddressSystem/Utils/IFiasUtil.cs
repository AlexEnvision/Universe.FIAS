using System.Collections.Generic;

namespace Universe.Fias.Normalizer.Algorithms.AddressSystem.Utils
{
	/// <summary>
	///		Утилита для работы с ФИАС
	/// </summary>
	public interface IFiasUtil
	{
        /// <summary>
        ///		Получает название региона без префикса и постфикса
        /// </summary>
        /// <param name="region">Название региона</param>
        /// <returns></returns>
        string GetRegionWithoutType(string region);

        /// <summary>
        ///		Получает название района без префикса и постфикса
        /// </summary>
        /// <param name="district">Название района</param>
        /// <returns></returns>
        string GetDistrictWithoutType(string district);

        /// <summary>
        ///		Получает название населенного пункта (города) без префикса и постфикса
        /// </summary>
        /// <param name="city">Название населенного пункта (города)</param>
        /// <returns></returns>
        string GetCityWithoutType(string city);

        /// <summary>
        ///		Получает название населенного пункта без префикса и постфикса
        /// </summary>
        /// <param name="locality">Название населенного пункта</param>
        /// <returns></returns>
        string GetLocationWithoutType(string locality);

        /// <summary>
        ///		Получает название улицы без префикса и постфикса
        /// </summary>
        /// <param name="street">Название улицы</param>
        /// <returns></returns>
        string GetStreetWithoutType(string street);

        /// <summary>
        ///		Разрешение полного названия типа адреса. 
        /// </summary>
        /// <param name="type">Разрешаемый тип</param>
        /// <returns></returns>
        string ResolveType(string type);

        /// <summary>
        ///		Разрешение нескольких полных названий типов адреса. 
        /// </summary>
        /// <param name="type">Разрешаемый тип</param>
        /// <returns></returns>
        List<string> ResolveTypes(string type);

        /// <summary>
        ///		Возвращает разобранный адрес, начинающийся с улицы
        /// </summary>
        /// <param name="address">Адрес, начинающийся с улицы</param>
        /// <returns>Разобранный адрес, или null если не удалось</returns>
        FiasInfo GetFiasInfo(string address);
	}
}
