using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace DropParty
{
    class TimeHandler
    {
        int CurrentTime = 0;
        float MaxTime = 0;
        public TimeHandler(float actionsPerSecond)
        {
            MaxTime = 60 / actionsPerSecond;
        }

        public bool Call()
        {
            CurrentTime++;
            bool returnValue = CurrentTime >= MaxTime;
            if (returnValue) { CurrentTime = 0; }
            return returnValue;
        }
        public void Delay()
        {
            if (CurrentTime - 1 < 0) CurrentTime = (int)MaxTime;
        }
    }
}