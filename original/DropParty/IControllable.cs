using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace DropParty
{
    public interface IControllable
    {
        string GetName();
        float GetMovement(List<IDummyCoin> coinList, List<IDummyPlayer> playerPosList, List<IDummyBullet> bulletPosList, List<IDummyBase> basePosList, int mapRadius);
        float GetFire(List<IDummyCoin> coinList, List<IDummyPlayer> playerPosList, List<IDummyBullet> bulletPosList, List<IDummyBase> basePosList, int mapRadius);
    }
}
