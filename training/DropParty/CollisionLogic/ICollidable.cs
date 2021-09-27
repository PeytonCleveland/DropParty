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
    public interface ICollidable
    {
        Vector2f Position { get; set; }

        Vector2f? RequestedPosition { get; set; }

        float Radius { get; }

        CollisionInfo CollisionInfo { get; } 

        void OnCollide(List<ICollidable> collisionList);
    }
}
