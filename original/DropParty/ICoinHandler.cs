using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace DropParty
{
    public interface ICoinHandler
    {
        void RemoveCoin(Coin coin);

        void PlayerDeath(List<Coin> heldCoins, Vector2f position);
    }
}
