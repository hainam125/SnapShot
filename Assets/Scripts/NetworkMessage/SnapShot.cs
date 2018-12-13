using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkMessage
{
    public class Entity
    {
        protected static long CurrentId = 1;
        protected long id;
        protected Entity()
        {
            id = CurrentId++;
        }
    }

    public class ExistingEntity : Entity
    {
        public CompressRotation rotation;
        public CompressPosition1 position;
    }

    public class DestroyedEntity : Entity
    {
    }

    public class NewEntity : Entity
    {
        public int prefabId;
    }

    public class SnapShot
    {
        public List<NewEntity> newEntities;
        public List<ExistingEntity> existingEntities;
        public List<DestroyedEntity> destroyedEntities;
        public long commandId;

        public SnapShot Clone()
        {
            return new SnapShot()
            {
                newEntities = newEntities,
                existingEntities = existingEntities,
                destroyedEntities = destroyedEntities,
                commandId = commandId
            };
        }
    }
}
