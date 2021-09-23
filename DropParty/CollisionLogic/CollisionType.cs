using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropParty
{
    public enum CollisionType
    {
        Enveloped = 0,
        InvertedEnveloped = 1,
        Touching = 2,
        InvertedTouching = 3,
        CenterContact = 4,
        InvertedCenterContact = 5
    }
}
