using System;
using System.ComponentModel.DataAnnotations;
using Universe.DataAccess.Models;
using Universe.Fias.DataAccess.Models.Base;

namespace Universe.Fias.DataAccess.Models
{
    public class AsHouse : Entity
    {
        public Guid HouseId { get; set; }

        public Guid AoId { get; set; }

        public int? DivType { get; set; }

        [MaxLength(24)]
        public string HouseNum { get; set; }

        public string BuildNum { get; set; }

        public string StrucNum { get; set; }

        public string PostalCode { get; set; }

        public string OKATO { get; set; }

        public string OKTMO { get; set; }

        public DateTime? UpdateDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string RegionCode { get; set; }

        public long? EstStatusId
        {
            get => GetEntityId(nameof(EstStatusId));
            set => SetEntityId(nameof(EstStatusId), value);
        }

        public virtual AsEstStatus EstStatus
        {
            get => GetEntity<AsEstStatus>(nameof(EstStatus));
            set => SetEntity(nameof(EstStatus), value);
        }

        public long? StrStatusId
        {
            get => GetEntityId(nameof(StrStatusId));
            set => SetEntityId(nameof(StrStatusId), value);
        }

        public virtual AsStrStatus StrStatus
        {
            get => GetEntity<AsStrStatus>(nameof(StrStatus));
            set => SetEntity(nameof(StrStatus), value);
        }
    }
}