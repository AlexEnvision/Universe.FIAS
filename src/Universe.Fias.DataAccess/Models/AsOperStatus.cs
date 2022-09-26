using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Universe.DataAccess.Models;

namespace Universe.Fias.DataAccess.Models
{
    public class AsOperStatus : Entity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AsOperStatus()
        {
            AsAddrObjs = new HashSet<AsAddrObj>();
        }

        public string Name { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AsAddrObj> AsAddrObjs { get; set; }

        public string Operstatid { get; set; }
    }
}