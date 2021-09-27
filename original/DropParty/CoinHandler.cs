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
    public class CoinHandler : ICoinHandler
    {
        public List<Coin> Coins { get; private set; }
        Vector2f MapCenter;
        float MapRadius;
        const int MaxCoins = 100;
        public CoinHandler(Vector2f mapCenter, float mapRadius)
        {
            MapCenter = mapCenter;
            MapRadius = mapRadius;
            Coins = new List<Coin>();

            GenerateCoins();
        }

        public void RemoveCoin(Coin coin)
        {
            Coins.Remove(coin);
        }

        public void Update()
        {
            GenerateCoins();
        }

        public void GenerateCoins()
        {

            while (Coins.Count < MaxCoins)
            {
                Vector2f pos = new Vector2f();
                int chance = Map.rng.Next(0, 99);
                int value = 1;
                if(chance < 5)
                {
                    value = 5;
                }
                else if (chance < 30)
                {
                    value = 2;
                }
                Coin coin = new Coin(this, new Vector2f(0, 0), value);
                do
                {
                    float radius = (float)(Map.rng.NextDouble() * ((MapRadius * .80F) /((value * 3) - 2)));
                    float radians = (float)(Map.rng.NextDouble() * 6);
                    float xPos = (float)(radius * Math.Cos(radians));
                    float yPos = (float)(radius * Math.Sin(radians));
                    coin.Position = new Vector2f(xPos + MapCenter.X, yPos + MapCenter.Y);
                } while (!CollisionHandler.IsValidPosition(coin.Position, coin));

                Coins.Add(coin);
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            Coins.ForEach(x => x.Draw(target, states));
        }

        public void PlayerDeath(List<Coin> heldCoins, Vector2f position)
        {
            foreach (var coin in heldCoins)
            {
                Vector2f pos = new Vector2f();
                do
                {
                    float radius = (float)(Map.rng.NextDouble() * 50);
                    float radians = (float)(Map.rng.NextDouble() * 6);

                    float xPos = (float)(radius * Math.Cos(radians));
                    float yPos = (float)(radius * Math.Sin(radians));

                    pos = new Vector2f(xPos + position.X, yPos + position.Y);
                } while (!CollisionHandler.IsValidPosition(pos, coin));
                coin.Position = pos;
                coin.isHeld = false;
            }
        }
    }
}