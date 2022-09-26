using System;
using System.Collections.Generic;
using Universe.DataAccess.Models;
using Universe.Helpers.Extensions;

namespace Universe.Fias.DataAccess.Models.Base
{
    public class UEntity : IUEntity
    {
        public Guid Id { get; set; }

        private readonly Dictionary<string, BaseForeignKeyEntity> _entityPropStorage = new Dictionary<string, BaseForeignKeyEntity>();

        public void SetUEntity<T>(string idPropName, T value) where T : UEntity
        {
            var entityProp = _entityPropStorage.GetOrCreate(idPropName, () => new ForeignKeyUEntity()) as ForeignKeyUEntity;
            if (entityProp != null)
            {
                entityProp.Entity = value;
                entityProp.Id = value?.Id;
            }
        }

        public T GetUEntity<T>(string idPropName) where T : UEntity
        {
            var entityProp = _entityPropStorage.GetOrCreate(idPropName, () => new ForeignKeyUEntity()) as ForeignKeyUEntity;
            return entityProp?.Entity as T;
        }

        public void SetEntityUId(string idPropName, Guid? value)
        {
            var entityProp = _entityPropStorage.GetOrCreate(idPropName, () => new ForeignKeyUEntity()) as ForeignKeyUEntity;
            if (entityProp != null)
            {
                entityProp.Id = value;
                if (entityProp.Id == null)
                    entityProp.Entity = null;

                if (entityProp.Entity?.Id != entityProp.Id)
                    entityProp.Entity = null;
            }
        }

        public Guid? GetEntityUId(string idPropName)
        {
            var entityProp = _entityPropStorage.GetOrCreate(idPropName, () => new ForeignKeyUEntity()) as ForeignKeyUEntity;
            if (entityProp?.Entity != null)
                return entityProp.Entity.Id;

            return entityProp?.Id;
        }

        public Guid GetEntityUIdNotNullable(string idPropName)
        {
            return GetEntityUId(idPropName) ?? Guid.Empty;
        }

        public void SetEntity<T>(string idPropName, T value) where T : Entity
        {
            var entityProp = _entityPropStorage.GetOrCreate(idPropName, () => new ForeignKeyEntity()) as ForeignKeyEntity;
            if (entityProp != null)
            {
                entityProp.Entity = value;
                entityProp.Id = value?.Id;
            }
        }

        public T GetEntity<T>(string idPropName) where T : Entity
        {
            var entityProp = _entityPropStorage.GetOrCreate(idPropName, () => new ForeignKeyEntity()) as ForeignKeyEntity;
            return entityProp?.Entity as T;
        }

        public void SetEntityId(string idPropName, long? value)
        {
            var entityProp = _entityPropStorage.GetOrCreate(idPropName, () => new ForeignKeyEntity()) as ForeignKeyEntity;
            if (entityProp != null)
            {
                entityProp.Id = value;
                if (entityProp.Id == null)
                    entityProp.Entity = null;

                if (entityProp.Entity?.Id != entityProp.Id)
                    entityProp.Entity = null;
            }
        }

        public long? GetEntityId(string idPropName)
        {
            var entityProp = _entityPropStorage.GetOrCreate(idPropName, () => new ForeignKeyEntity()) as ForeignKeyEntity;
            if (entityProp?.Entity != null)
                return entityProp.Entity.Id;

            return entityProp?.Id;
        }

        public long GetEntityIdNotNullable(string idPropName)
        {
            return GetEntityId(idPropName) ?? 0;
        }

        public class BaseForeignKeyEntity
        {
        }

        public class BaseForeignKeyEntity<T> : BaseForeignKeyEntity
        {
            public T Entity { get; set; }
        }

        public class ForeignKeyEntity : BaseForeignKeyEntity<Entity>
        {
            public long? Id { get; set; }
        }

        public class ForeignKeyUEntity : BaseForeignKeyEntity<UEntity>
        {
            public Guid? Id { get; set; }
        }
    }
}