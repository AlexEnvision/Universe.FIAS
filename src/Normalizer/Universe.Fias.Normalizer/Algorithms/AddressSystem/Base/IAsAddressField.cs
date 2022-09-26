using Universe.Fias.Normalizer.Algorithms.AddressSystem.Settings;
using Universe.Fias.Normalizer.Models.AddressSystem.Field;

namespace Universe.Fias.Normalizer.Algorithms.AddressSystem.Base
{
    /// <summary>
    ///     Address system interface field.
    /// </summary>
    public interface IAsAddressField
    {
        /// <summary>
        ///     Получает, либо устанавливает настройки.
        ///     Gets or sets the settings.
        /// </summary>
        /// <value>
        ///     Настройки.
        ///     The settings.
        /// </value>
        AsAddressFieldSettings Settings { get; set; }

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
        AsAddressFieldValue BuildValue(AddrCodes codes, BuildAddressSettings settings = null);

        /// <summary>
        ///     Получает значение поля как текстовое.
        ///     Gets the field value object as text.
        /// </summary>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <returns></returns>
        string GetFieldValueObjAsText(AsAddressFieldValue value);
    }
}