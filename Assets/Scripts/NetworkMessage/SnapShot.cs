using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkMessage
{
    [System.Serializable]
    public class Entity
    {
        protected static long CurrentId = 1;
        public long id;
        protected Entity()
        {
            id = CurrentId++;
        }
    }
    [System.Serializable]
    public class ExistingEntity : Entity
    {
        public CompressRotation rotation;
        public CompressPosition2 position;
    }

    [System.Serializable]
    public class DestroyedEntity : Entity
    {
    }

    [System.Serializable]
    public class NewEntity : Entity
    {
        public int prefabId;
        public CompressRotation rotation;
        public CompressPosition2 bound;
        public CompressPosition2 position;
    }

    [System.Serializable]
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
