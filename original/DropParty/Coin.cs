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
    public class Coin : ICollidable, IDummyCoin
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

        public CollisionInfo CollisionInfo { get; private set; } = new CollisionInfo(new List<CollisionInstance>()
        {
            new CollisionInstance(typeof(Player), CollisionType.Touching)
        });

        public float Radius { get => myCirc.Radius; set => myCirc.Radius = value; }

        ICoinHandler CoinHandler;

        public int Value { get; private set; }

        private bool held = true;

        public bool isHeld
        {
            get => held; set
            {
                if(held != value)
                {
                    if (!value)
                    {
                        held = value;
                        //CollisionHandler.Add(this);
                    }
                    else
                    {
                        held = value;
                        //CollisionHandler.Remove(this);
                    }
                }
            }
        }


        CircleShape myCirc;

        public Coin(ICoinHandler coinHandler, Vector2f position, int value)
        {
            CoinHandler = coinHandler;
            Value = value;
            Color color;
            CollisionHandler.Add(this);

            switch (value)
            {
                case 1: color = Color.Yellow; break;
                case 2: color = Color.Red; break;

                case 3: 
                case 4:
                  
                default: 
                    color = Color.Blue; break;
            }

            myCirc = new CircleShape()
            {
                FillColor = color
            };

            Position = position;
            Radius = 4;
            isHeld = false;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (!isHeld)
            {
                myCirc.Draw(target, states);
            }
        }

        public void RemoveFromHandler()
        {
            CollisionHandler.Remove(this);
            CoinHandler.RemoveCoin(this);
        }

        public void OnCollide(List<ICollidable> collidedType)
        {
        }
        public IDummyCoin DeepCopy()
        {
            FakeCoin newCoin = new FakeCoin();
            newCoin.Position = new Vector2f(Position.X, Position.Y);
            newCoin.Value = Value;
            newCoin.isHeld = isHeld;
            newCoin.Radius = Radius;
            return newCoin;
        }
    }
    public class FakeCoin : IDummyCoin
    {
        public Vector2f Position { get; set; }
        public int Value { get; set; }

        public bool isHeld { get; set; }
        public float Radius { get; set; }
        public IDummyCoin DeepCopy() => null;
    }
}