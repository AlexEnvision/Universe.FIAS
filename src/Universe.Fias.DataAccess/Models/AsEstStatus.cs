using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Universe.DataAccess.Models;

namespace Universe.Fias.DataAccess.Models
{
    /// <summary>
    ///     Статус дома
    /// </summary>
    public class AsEstStatus : Entity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AsEstStatus()
        {
            AsHouses = new HashSet<AsHouse>();
        }

        public string Name { get; set; }
        public string ShortName { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AsHouse> AsHouses { get; set; }

        public string Eststatid { get; set; }
    }
}