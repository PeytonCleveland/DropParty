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
    public class Base : ICollidable, IBase, IDummyBase
    {
        public Color myColor;
        public Vector2f Position
        {
            get => myCirc.Position + new Vector2f(Radius, Radius);
            set
            {
                value -= new Vector2f(Radius, Radius);
                myCirc.Position = value;
            }
        }
        public string PlayerName { get; private set; }

        public void SetName(string name)
        {
            PlayerName = name;
        }
        public Vector2f? RequestedPosition { get; set; }

        public CollisionInfo CollisionInfo { get; private set; } = new CollisionInfo(new List<CollisionInstance>()
        {
            new CollisionInstance(typeof(Player), CollisionType.Enveloped),
            new CollisionInstance(typeof(Bullet), CollisionType.Touching),
            new CollisionInstance(typeof(Coin), CollisionType.Touching)
        });

        public float Radius { get => myCirc.Radius; set => myCirc.Radius = value; }

        CircleShape myCirc;

        public Base(Vector2f position)
        {
            byte[] rgb = new byte[3];
            Map.rng.NextBytes(rgb);
            Color color = new Color(rgb[0], rgb[1], rgb[2]);
            myColor = color;
            myCirc = new CircleShape()
            {
                FillColor = color
            };

            Radius = 20;
            Position = position;
            CollisionHandler.Add(this);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            myCirc.Draw(target, states);
        }

        public void OnCollide(List<ICollidable> collidedType)
        {

        }
        public IDummyBase DeepCopy()
        {
            FakeBase newBase = new FakeBase();
            newBase.Position = new Vector2f(Position.X, Position.Y);
            newBase.PlayerName = PlayerName;
            newBase.Radius = Radius;
            return newBase;

        }
    }
    public class FakeBase : IDummyBase
    {
        public Vector2f Position { get; set; }
        public string PlayerName { get; set; }
        public float Radius { get; set; }
        public IDummyBase DeepCopy() => null;
    }
}