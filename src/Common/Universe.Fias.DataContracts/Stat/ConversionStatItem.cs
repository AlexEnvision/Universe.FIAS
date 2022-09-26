using System.Collections.Generic;

namespace Universe.Fias.DataContracts.Stat
{
    public class ConversionStatItem
    {
        public long Count { get; set; }
    }

    public class ConversionStatItem<T> : ConversionStatItem
    {
        public IList<T> Records { get; set; }
    }
}