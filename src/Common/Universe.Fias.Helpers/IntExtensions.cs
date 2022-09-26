namespace Universe.Fias.Helpers
{
    public static class IntExtensions
    {
        public static int? TryParseNullable(this string val)
        {
            int outValue;
            return int.TryParse(val, out outValue) ? (int?) outValue : null;
        }
    }
}