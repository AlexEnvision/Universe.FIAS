using System;
using System.Collections.Generic;
using System.Linq;
using Universe.Helpers.Extensions;

namespace Universe.Fias.Helpers
{
    public static class StringExtensions
    {
        public static Guid ToGuidThrowIfNull(this string guidSfy)
        {
            if (guidSfy.IsNullOrEmpty())
                throw new ArgumentNullException(@$"{nameof(guidSfy)} == null");

            if (!Guid.TryParse(guidSfy, out var value))
            {
                throw new ArgumentNullException(@$"{nameof(guidSfy)} hasn't parsed");
            }

            return value;
        }

        public static Guid? ToGuidTryParse(this string guidSfy)
        {
            if (!Guid.TryParse(guidSfy, out var value))
            {
                return null;
            }

            return value;
        }

        /// <summary>
        ///		Вырезает из начала строки
        /// </summary>
        /// <param name="value">Строка для обрезки</param>
        /// <param name="trimValues">Значения обрезки</param>
        /// <param name="addSpace"></param>
        /// <returns></returns>
        public static string TrimStart(this string value, IEnumerable<string> trimValues, bool addSpace = true)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            foreach (var trimValue in trimValues.OrderByDescending(v => v.Length))
            {
                var startValue = addSpace ? trimValue.Trim() + " " : trimValue.Trim();
                if (value.StartsWith(startValue, System.StringComparison.OrdinalIgnoreCase))
                {
                    value = value.Substring(startValue.Length);
                    break;
                }
            }

            return value;
        }

        /// <summary>
        ///		Вырезает из начала строки
        /// </summary>
        /// <param name="value">Строка для обрезки</param>
        /// <param name="trimValues">Значения обрезки</param>
        /// <param name="addSpace"></param>
        /// <returns></returns>
        public static string TrimEnd(this string value, IEnumerable<string> trimValues, bool addSpace = true)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            foreach (var trimValue in trimValues.OrderByDescending(v => v.Length))
            {
                var endValue = addSpace ? " " + trimValue.Trim() : trimValue.Trim();
                if (value.EndsWith(endValue, System.StringComparison.OrdinalIgnoreCase))
                {
                    value = value.Substring(0, value.Length - endValue.Length);
                    break;
                }
            }

            return value;
        }
    }
}
