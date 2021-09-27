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
    public class PlayerHandler
    {
        List<Player> Players;

        public PlayerHandler(IMap map, List<Type> listOfPlayers, List<Base> bases)
        {
            Players = new List<Player>();
            for(int i = 0; i < listOfPlayers.Count; i++)
            {
                Player newPlayer =  new Player(map, bases[i], (IControllable)Activator.CreateInstance(listOfPlayers[i], i), bases[i].myColor, this);
                Players.Add(newPlayer);
            }
        }
        public List<Player> GetPlayers { get => Players; }

        public void Update(List<IDummyCoin> dummyCoins, List<IDummyBullet> dummyBullets, List<IDummyBase> dummyBases, int mapRadius)
        {
            Players.ForEach(x => x.Update(dummyCoins.ToList<IDummyCoin>(), Players.ToList<IDummyPlayer>(), dummyBullets.ToList<IDummyBullet>(), dummyBases.ToList<IDummyBase>(), mapRadius));
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            Players.ForEach(x => x.Draw(target, states));
        }

        public void UpdatePlayerKills(string shootingPlayerName)
        {
            foreach (Player player in Players)
            {
                if (player.PlayerName == shootingPlayerName)
                {
                    player.playerKills++;
                }
            }
        }
    }
}
