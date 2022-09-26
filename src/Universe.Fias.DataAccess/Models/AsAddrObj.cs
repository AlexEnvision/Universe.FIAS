using System;
using System.ComponentModel.DataAnnotations;
using Universe.DataAccess.Models;
using Universe.Fias.DataAccess.Models.Base;

namespace Universe.Fias.DataAccess.Models
{
    public class AsAddrObj : Entity
    {
        public Guid AoId { get; set; }

        public Guid? ParentAoId { get; set; }

        public int? DivType { get; set; }

        public int? AoLevel { get; set; }

        public string TypeShortName { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        public string OffName { get; set; }

        [MaxLength(20)]
        public string Code { get; set; }

        [MaxLength(20)]
        public string PlainCode { get; set; }

        public string PostalCode { get; set; }

        public string OKATO { get; set; }

        public string OKTMO { get; set; }

        public int LiveStatusId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string TypeCode { get; set; }

        [MaxLength(20)]
        public string AoCode { get; set; }

        public long? ActStatusId
        {
            get => GetEntityId(nameof(ActStatusId));
            set => SetEntityId(nameof(ActStatusId), value);
        }

        public virtual AsActStatus ActStatus
        {
            get => GetEntity<AsActStatus>(nameof(ActStatus));
            set => SetEntity(nameof(ActStatus), value);
        }

        public long? CentStatusId 
        {
            get => GetEntityId(nameof(CentStatusId));
            set => SetEntityId(nameof(CentStatusId), value);
        }

        public virtual AsCenterStatus CenterStatus
        {
            get => GetEntity<AsCenterStatus>(nameof(CenterStatus));
            set => SetEntity(nameof(CenterStatus), value);
        }

        public long? OperStatusId
        {
            get => GetEntityId(nameof(OperStatusId));
            set => SetEntityId(nameof(OperStatusId), value);
        }

        public virtual AsOperStatus OperStatus
        {
            get => GetEntity<AsOperStatus>(nameof(OperStatus));
            set => SetEntity(nameof(OperStatus), value);
        }

        public Guid? NextId { get; set; }
        //{
        //    get => GetEntityUId(nameof(NextId));
        //    set => SetEntityUId(nameof(NextId), value);
        //}

        //public virtual AsAddrObj NextVer
        //{
        //    get => GetUEntity<AsAddrObj>(nameof(NextVer));
        //    set => SetUEntity(nameof(NextVer), value);
        //}

        public Guid? PrevId { get; set; }
        //{
        //    get => GetEntityUId(nameof(PrevId));
        //    set => SetEntityUId(nameof(PrevId), value);
        //}

        //public virtual AsAddrObj PrevVer
        //{
        //    get => GetUEntity<AsAddrObj>(nameof(PrevVer));
        //    set => SetUEntity(nameof(PrevVer), value);
        //}

        public virtual long? TypeId
        {
            get => GetEntityId(nameof(TypeId));
            set => SetEntityId(nameof(TypeId), value);
        }

        public virtual AsAddrObjType Type
        {
            get => GetEntity<AsAddrObjType>(nameof(Type));
            set => SetEntity(nameof(Type), value);
        }
    }
}