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
    public interface IBase
    {     
        Vector2f Position { get; }
        string PlayerName { get; }
        void SetName(string name);
    }
}