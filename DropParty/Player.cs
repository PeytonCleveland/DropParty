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
    public class Player : IPlayer, ICollidable, IDummyPlayer
    {
        private Text myNameText;
        public IControllable Controllable { get; set; }
        public string PlayerName { get; private set; }
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
            new CollisionInstance(typeof(Bullet), CollisionType.Touching),
            new CollisionInstance(typeof(Player), CollisionType.Touching)
        });


        public float Radius { get => myCirc.Radius; set => myCirc.Radius = value; }
        public IBase MyBase { get; set; }

        public int HeldValue => HeldCoins.Sum(x => x.Value);

        public int BankedValue => BankedCoins.Sum(x => x.Value);

        List<Coin> HeldCoins;
        List<Coin> BankedCoins;
        TimeHandler bulletTime = new TimeHandler(1);

        IMap myMapRef;

        CircleShape myCirc;

        float PlayerSpeed;
        float BulletSpeed;

        public Player(IMap mapBullets, IBase Base, IControllable controllable, Color baseColor)
        {
            myNameText = new Text("", new Font(@"Resources/sansation.ttf"), 13);
            Controllable = controllable;
            MyBase = Base;
            if (Controllable != null) { MyBase.SetName(controllable.GetName()); }
            this.myMapRef = mapBullets;
            myCirc = new CircleShape();
            myCirc.FillColor = baseColor;
            Radius = 10;
            HeldCoins = new List<Coin>();
            BankedCoins = new List<Coin>();
            PlayerSpeed = 3;
            BulletSpeed = 50;
            if (controllable != null) { PlayerName = Controllable.GetName(); }
            if (controllable != null) { myNameText.DisplayedString = Controllable.GetName(); }
            CollisionHandler.Add(this);

            Spawn();
        }


        public void Update(List<IDummyCoin> dummyCoins, List<IDummyPlayer> dummyPlayers, List<IDummyBullet> dummyBullets, List<IDummyBase> dummyBases, int mapRadius)
        {
            List<IDummyCoin> coinList = new List<IDummyCoin>();
            foreach (IDummyCoin c in dummyCoins) { coinList.Add(c.DeepCopy()); }

            List<IDummyPlayer> playerList = new List<IDummyPlayer>();
            foreach (IDummyPlayer c in dummyPlayers) { playerList.Add(c.DeepCopy()); }

            List<IDummyBullet> bulletList = new List<IDummyBullet>();
            foreach (IDummyBullet c in dummyBullets) { bulletList.Add(c.DeepCopy()); }

            List<IDummyBase> baseList = new List<IDummyBase>();
            foreach (IDummyBase c in dummyBases) { baseList.Add(c.DeepCopy()); }

            if (Controllable != null)
            {
                float thisMove = -1;
                try
                {
                    thisMove = Controllable.GetMovement(coinList, playerList, bulletList, baseList, mapRadius);
                }
                catch (Exception e)
                {
                    Debug.Assign($"{PlayerName} Crashed", "Move function crashed! Oh no! \n" + e.Message);
                }

                if (thisMove >= 0)
                {
                    Move(thisMove);
                }
                if(bulletTime.Call())
                {
                    float thisShoot = -1;
                    try
                    {
                        thisShoot = Controllable.GetFire(coinList, playerList, bulletList, baseList, mapRadius);
                    }
                    catch (Exception e)
                    {
                        Debug.Assign($"{PlayerName} Crashed", "Shoot function crashed! Oh no! \n" + e.Message);
                    }

                    if(thisShoot >= 0)
                    {
                        Shoot(thisShoot);
                    }
                    else
                    {
                        bulletTime.Delay();
                    }
                }
            }
            else
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                {
                    Move(180);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                {
                    Move(0);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                {
                    Move(270);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                {
                    Move(90);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
                {
                    Shoot(270);
                }
            }
            myNameText.Position = new Vector2f(Position.X - myNameText.GetLocalBounds().Width / 2, Position.Y - 24);
            Debug.Assign($"{PlayerName} Held Value:", HeldCoins.Sum(x => x.Value));
            Debug.Assign($"{PlayerName} Banked Value:", BankedCoins.Sum(x => x.Value));
            Debug.Assign($"{PlayerName} Held Count:", HeldCoins.Count);
            Debug.Assign($"{PlayerName} Banked Count:", BankedCoins.Count);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            myCirc.Draw(target, states);
            myNameText.Draw(target, states);
        }

        public void OnCollide(List<ICollidable> collidedType)
        {
            bool doMove = true;

            foreach (var collision in collidedType)
            {
                if (collision is Bullet || collision is Player || collision is Map)
                {
                    Spawn();
                    doMove = false;
                    break;
                }
                if (collision is Coin coin && !coin.isHeld)
                {
                    coin.isHeld = true;
                    HeldCoins.Add(coin);
                    collision.OnCollide(new List<ICollidable>() { this });
                }
                if (collision is Base Base)
                {
                    if (Base == MyBase)
                    {
                        BankCoins();
                    }
                    else
                    {
                        Spawn();
                        doMove = false;
                        break;
                    }
                }
            }

            if (doMove)
            {
                Position = (Vector2f)RequestedPosition;
            }
        }

        public Type GetCollideType()
        {
            return typeof(Player);
        }

        public ICollidable GetCollidable()
        {
            return this;
        }

        public void BankCoins()
        {
            BankedCoins.AddRange(HeldCoins);
            HeldCoins.ForEach(x => x.RemoveFromHandler());
            HeldCoins.Clear();
        }

        public void Shoot(float degrees)
        {
            float radians = Rad(degrees);
            Vector2f myVelocity = new Vector2f((float)Math.Cos(radians) * 10, (float)Math.Sin(radians) * 10);
            Bullet myBullet = new Bullet(myVelocity, myMapRef, (int)BulletSpeed);
            myBullet.Position = new Vector2f((float)Math.Cos(radians) * (Radius + myBullet.Radius + 2 + PlayerSpeed), (float)Math.Sin(radians) * (Radius + myBullet.Radius + 2 + PlayerSpeed)) + (RequestedPosition ?? Position);
            myMapRef.BulletList.Add(myBullet);
        }

        public void Move(float degrees)
        {
            float radians = Rad(degrees);
            Vector2f myVelocity = new Vector2f((float)Math.Cos(radians) * PlayerSpeed, (float)Math.Sin(radians) * PlayerSpeed);

            RequestedPosition = Position + myVelocity;
        }

        public void Spawn()
        {
            myMapRef.GetCoinHandler.PlayerDeath(HeldCoins, Position);
            HeldCoins.Clear();
            myCirc.Position = MyBase.Position - new Vector2f(myCirc.Radius, myCirc.Radius);
        }

        private float Rad(float degrees)
        {
            return (float)(degrees * (Math.PI / 180F));
        }
        public IDummyPlayer DeepCopy()
        {
            FakePlayer newPlayer = new FakePlayer();
            newPlayer.Position = new Vector2f(Position.X, Position.Y);
            newPlayer.PlayerName = PlayerName;
            newPlayer.Radius = Radius;
            newPlayer.HeldValue = HeldValue;
            newPlayer.BankedValue = BankedValue;
            return newPlayer;
        }
    }
    public class FakePlayer : IDummyPlayer
    {
        public Vector2f Position { get; set; }
        public string PlayerName { get; set; }
        public float Radius { get; set; }

        public int HeldValue { get; set; }

        public int BankedValue { get; set; }
        public IDummyPlayer DeepCopy() => null;
    }
}
