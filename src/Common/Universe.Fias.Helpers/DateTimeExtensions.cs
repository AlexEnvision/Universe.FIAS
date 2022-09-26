using System;

namespace Universe.Fias.Helpers
{
    public static class DateTimeExtensions
    {
        public static DateTime? TryParseNullable(this string val)
        {
            DateTime outValue;
            return DateTime.TryParse(val, out outValue) ? (DateTime?)outValue : null;
        }
    }
}
