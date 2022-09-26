using System;
using Universe.DataAccess.Models;

namespace Universe.Fias.DataAccess.Models
{
    public class AsVersionFile : Entity
    {
        public string FileName { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? StartDateProcessing { get; set; }

        public DateTimeOffset? EndDateProcessed { get; set; }

        public bool? IsProcessed { get; set; }

        public string Error { get; set; }

        public long? VersionId
        {
            get => GetEntityId(nameof(VersionId));
            set => SetEntityId(nameof(VersionId), value);
        }

        public virtual AsVersion AsVersion
        {
            get => GetEntity<AsVersion>(nameof(AsVersion));
            set => SetEntity(nameof(AsVersion), value);
        }
    }
}