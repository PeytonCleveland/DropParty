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
    public class Bullet : ICollidable, IDummyBullet
    {
        public Vector2f Position
        {
            get => myCirc.Position + new Vector2f(Radius, Radius);
            set
            {
                value -= new Vector2f(Radius, Radius);
                myCirc.Position = value;
            }
        }

        public Vector2f? RequestedPosition { get; set; }

        public Vector2f Velocity { get; private set; }

        public float Radius { get => myCirc.Radius; set => myCirc.Radius = value; }

        CircleShape myCirc;

        public CollisionInfo CollisionInfo { get; private set; } = new CollisionInfo(new List<CollisionInstance>()
        {
            new CollisionInstance(typeof(Player), CollisionType.Touching)
        });

        IMap map;

        TimeHandler timeHandler;

        int BulletSpeed;

        public string ShootingPlayerName { get; set; }
        
        public Bullet(Vector2f velocity, IMap map, int bulletSpeed, string shootingPlayerName)
        {
            myCirc = new CircleShape();
            Radius = 2;
            Velocity = velocity;
            this.map = map;
            myCirc.FillColor = Color.Red;
            BulletSpeed = bulletSpeed;
            timeHandler = new TimeHandler(BulletSpeed);
            CollisionHandler.Add(this);
            ShootingPlayerName = shootingPlayerName;
        }

        public void Update()
        {
            if (timeHandler.Call())
            {
                RequestedPosition = Position + Velocity;
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            myCirc.Draw(target, states);
        }

        public void OnCollide(List<ICollidable> collidedType)
        {
            if(collidedType.Any(x => x is Base || x is Map))
            {
                map.DestroyItem(this);
                CollisionHandler.Remove(this);
            }
            else if(collidedType.Any(x => x is Player))
            {
                collidedType.First(x => x is Player).OnCollide(new List<ICollidable>() { this });
                map.DestroyItem(this);
                CollisionHandler.Remove(this);
            }

            Position = (Vector2f)RequestedPosition;
        }

        public Type GetCollideType()
        {
            return typeof(Player);
        }
        public IDummyBullet DeepCopy()
        {
            FakeBullet newBullet = new FakeBullet();
            newBullet.Position = new Vector2f(Position.X, Position.Y);
            newBullet.Velocity = new Vector2f(Velocity.X, Velocity.Y);
            newBullet.Radius = Radius;
            return newBullet;
        }
    }
    public class FakeBullet : IDummyBullet
    {
        public Vector2f Position { get; set; }
        public Vector2f Velocity { get; set; }
        public float Radius { get; set; }
        public IDummyBullet DeepCopy() => null;
    }
}