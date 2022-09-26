using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Universe.DataAccess.Models;

namespace Universe.Fias.DataAccess.Models
{
    /// <summary>
    ///     Статус улицы
    /// </summary>
    public class AsStrStatus : Entity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AsStrStatus()
        {
            AsHouses = new HashSet<AsHouse>();
        }

        public string Name { get; set; }
        public string ShortName { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AsHouse> AsHouses { get; set; }

        public string Strstatid { get; set; }
    }
}