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
    public interface IMap
    {
        List<Bullet> BulletList { get; set; }
        ICoinHandler GetCoinHandler { get; }
        void DestroyItem(object item);

    }
}