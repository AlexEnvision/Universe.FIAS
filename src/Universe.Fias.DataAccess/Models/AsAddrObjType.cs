using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Universe.DataAccess.Models;

namespace Universe.Fias.DataAccess.Models
{
    public class AsAddrObjType : Entity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AsAddrObjType()
        {
            AddrObjs = new HashSet<AsAddrObj>();
        }

        public string Code { get; set; }

        public int? Level { get; set; }

        public string ShortName { get; set; }

        public string Name { get; set; }

        public bool? ThereAreStreets { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AsAddrObj> AddrObjs { get; set; }
    }
}