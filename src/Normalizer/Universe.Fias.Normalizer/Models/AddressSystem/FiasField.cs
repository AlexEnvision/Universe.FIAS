namespace Universe.Fias.Normalizer.Models.AddressSystem
{
    /// <summary>
    /// The сlass storing the values of the Kladr Address field
    /// </summary>
    public class FiasField
    {
        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// District
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// House
        /// </summary>
        public string House { get; set; }

        /// <summary>
        /// Locality
        /// </summary>
        public string Locality { get; set; }

        /// <summary>
        /// Region
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Street
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Number of the row
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Check for an all empty field
        /// </summary>
        public bool IsNullOrEmpty => City == null && District == null && 
            House == null && Locality == null && Region == null && Street == null;
    }
}