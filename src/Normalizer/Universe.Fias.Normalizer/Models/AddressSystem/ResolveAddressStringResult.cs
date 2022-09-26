using Universe.Fias.Normalizer.Models.AddressSystem.Field;

namespace Universe.Fias.Normalizer.Models.AddressSystem
{
    /// <summary>
    ///     Результат операции разрешения.
    /// </summary>
    public class ResolveAddressStringResult
    {
        /// <summary>
        ///     Адресные коды
        /// </summary>
        public AddrCodes Codes { get; set; }

        /// <summary>
        ///     Разрешённые значения.
        /// </summary>
        public AsAddressFieldValue Result { get; set; }
        
        /// <summary>
        ///     Возвращает пустой результат.
        /// </summary>
        public static ResolveAddressStringResult EmptyResult => new ResolveAddressStringResult
        {
            Codes = new AddrCodes(),
            Result = new AsAddressFieldValue()
        };
    }
}