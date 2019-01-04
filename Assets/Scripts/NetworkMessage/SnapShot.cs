using System.Collections;
using System.Collections.Generic;

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
        public int prefabId;
        public CompressRotation rotation;
        public CompressPosition2 position;
    }

    [System.Serializable]
    public class ExistingPlayer : ExistingEntity
    {
        public int hp;
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
    public class NewPlayer : NewEntity
    {
        public int hp;
    }

    [System.Serializable]
    public class SnapShot
    {
        public List<NewEntity> newEntities;
        public List<NewPlayer> newPlayers;
        public List<ExistingEntity> existingEntities;
        public List<ExistingPlayer> existingPlayers;
        public List<DestroyedEntity> destroyedEntities;
        public long commandId;

        public SnapShot Clone()
        {
            return new SnapShot()
            {
                newEntities = newEntities,
                newPlayers = newPlayers,
                existingEntities = existingEntities,
                existingPlayers = existingPlayers,
                destroyedEntities = destroyedEntities,
                commandId = commandId
            };
        }
    }
}
