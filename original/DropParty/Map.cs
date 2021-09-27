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
    public class Map : IMap, ICollidable
    {
        float Size;

        public static Random rng = new Random();
        CoinHandler CoinHandler;
        PlayerHandler PlayerHandler;
        CircleShape MapCircle;
        List<Base> Bases;

        public Vector2f Position
        {
            get => MapCircle.Position + new Vector2f(Radius, Radius);
            set
            {
                value -= new Vector2f(Radius, Radius);
                MapCircle.Position = value;
            }
        }

        public Vector2f? RequestedPosition { get; set; }

        public float Radius { get => MapCircle.Radius; set => MapCircle.Radius = value; }

        public CollisionInfo CollisionInfo { get; private set; } = new CollisionInfo(new List<CollisionInstance>()
        {
            new CollisionInstance(typeof(Bullet), CollisionType.InvertedEnveloped),
            new CollisionInstance(typeof(Player), CollisionType.InvertedEnveloped),
            new CollisionInstance(typeof(Coin), CollisionType.InvertedEnveloped)
        });

        public void OnCollide(List<ICollidable> collidable) { }

        public List<Bullet> BulletList { get; set; } = new List<Bullet>();
        public ICoinHandler GetCoinHandler { get => CoinHandler; }
        public Map(List<Type> listOfPlayers)
        {
            int numberOfPlayers = listOfPlayers.Count;
            CollisionHandler.Add(this);

            Size = 100 + (120 * numberOfPlayers);

            MapCircle = new CircleShape(Size, 30 + (uint)(numberOfPlayers * 10))
            {
                FillColor = new Color(10, 10, 10),
                OutlineColor = new Color(50, 50, 50),
            };

            CoinHandler = new CoinHandler(new Vector2f(Radius, Radius), Radius);
       
            Bases = new List<Base>();

            for (int i = 0; i < numberOfPlayers; i++)
            {
                float radiansSpacing = (float)(2 * Math.PI / numberOfPlayers);
                float xOffSet = (float)((Size * .90F) * Math.Cos(radiansSpacing * i));
                float yOffSet = (float)((Size * .90F) * Math.Sin(radiansSpacing * i));

                Vector2f basePosition = new Vector2f(Size + xOffSet, Size + yOffSet);

                Base baseToAdd = new Base(basePosition);
                Bases.Add(baseToAdd);
            }
            PlayerHandler = new PlayerHandler(this, listOfPlayers, Bases);

        }

        public void Update()
        {
            CoinHandler.Update();
            BulletList.ForEach(x => x.Update());
            PlayerHandler.Update(CoinHandler.Coins.ToList<IDummyCoin>(), BulletList.ToList<IDummyBullet>(), Bases.ToList<IDummyBase>(), (int)Radius);
       
            CollisionHandler.Update();
        }

        public void DestroyItem(object item)
        {
            if(item is Bullet)
            {
                BulletList.Remove((Bullet)item);
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            //Regardless of the size of the circle it will fit to screen
            Vector2f scaleFactor = new Vector2f(GameLoop.myWindow.Size.Y / (MapCircle.Radius * 2), GameLoop.myWindow.Size.Y / (MapCircle.Radius * 2));
            states.Transform.Scale(scaleFactor);

            MapCircle.Draw(target, states);
            states.Transform.Translate(MapCircle.Position);

            Bases.ForEach(x => x.Draw(target, states));
            CoinHandler.Draw(target, states);
            PlayerHandler.Draw(target, states);
            BulletList.ForEach(x => x.Draw(target, states));
        }
    }
}
