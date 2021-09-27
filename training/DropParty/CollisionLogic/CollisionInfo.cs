using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace DropParty
{
    public class CollisionInfo
    {
        public CollisionInfo(List<CollisionInstance> collisionInstances)
        {
            CollisionInstances = collisionInstances;
        }

        public List<CollisionInstance> CollisionInstances { get; set; } = new List<CollisionInstance>();

        public bool IsCollisionCheckNeeded(ICollidable type)
        {
            return CollisionInstances.Any(x => x.Type == type.GetType());
        }
        public bool IsCollisionCheckNeeded(CollisionType collisionType)
        {
            return CollisionInstances.Any(x => x.CollisionType == collisionType);
        }

        public bool IsCollisionCheckNeeded(CollisionType collisionType, ICollidable type)
        {
            return CollisionInstances.Any(x => x.CollisionType == collisionType && x.Type == type.GetType());
        }
    }

    public class CollisionInstance
    {
        public CollisionInstance(Type type, CollisionType collisionType)
        {
            CollisionType = collisionType;
            Type = type;
        }

        public CollisionType CollisionType;
        public Type Type;
    }
}
