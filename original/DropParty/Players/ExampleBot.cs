using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace DropParty
{
    public class ExampleBot : IControllable
    {
        Random Rng;
        string Name;
        /// <summary>
        /// This is the initialization method. Set your string 'Name' to whatever you want your bot to be called.
        /// If you do not add 'myNum' to the end of your string, I will do it. This ensures that two bots with the same name will still work correctly.
        /// Your instructor MUST take an int. Unfortunately interfaces can't standardize the constructor signature, so just try not to forget.
        /// </summary>
        /// <param name="myNum">This number is your bot's "index".</param>
        public ExampleBot(int myNum)
        {
            Rng = new Random();
            Name = "ExampleBot" + myNum.ToString();
        }
        /// <summary>
        /// This method will be called every frame to ask your robot if it wants to fire, and what direction.
        /// Return -1 to not fire. Your shot will not go on cooldown if you do not fire. You can only shoot once per second.
        /// </summary>
        /// <param name="coinList">List of coins on the map</param>
        /// <param name="playerList">List of players on the map</param>
        /// <param name="bulletList">List of bullets on the map</param>
        /// <param name="baseList">List of bases on the map</param>
        /// <param name="mapRadius">The radius of the map</param>
        /// <returns>Returns the direction to fire in degrees</returns>
        public float GetFire(List<IDummyCoin> coinList, List<IDummyPlayer> playerList, List<IDummyBullet> bulletList, List<IDummyBase> baseList, int mapRadius)
        {
            return -1;// (float)Rng.NextDouble() * 360;
        }
        /// <summary>
        /// This method will be called every frame to ask your robot if it wants to move, and in what direction.
        /// Return -1 to not move.
        /// </summary>
        /// <param name="coinList">List of coins on the map</param>
        /// <param name="playerList">List of players on the map</param>
        /// <param name="bulletList">List of bullets on the map</param>
        /// <param name="baseList">List of bases on the map</param>
        /// <param name="mapRadius">The radius of the map</param>
        /// <returns>Returns the direction to move in degrees</returns>
        public float GetMovement(List<IDummyCoin> coinList, List<IDummyPlayer> playerList, List<IDummyBullet> bulletList, List<IDummyBase> baseList, int mapRadius)
        {
            IDummyPlayer me = GetMe(playerList);
            coinList.RemoveAll(coin => coin.isHeld);
            if (coinList.Count > 0)
            {
                //Finds closest coin
                coinList = coinList.OrderBy(coin => GetPythag(coin.Position, me.Position)).ToList<IDummyCoin>();
                // Goes towards it
                return FindAngleToTarget(me.Position, coinList[0].Position);
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// This method gets the distance between two objects.
        /// </summary>
        /// <param name="locationOne"></param>
        /// <param name="locationTwo"></param>
        /// <returns>Distance from first location to the second</returns>
        private float GetPythag(Vector2f locationOne, Vector2f locationTwo)
        {
            return (float)Math.Sqrt(Math.Pow(locationOne.X - locationTwo.X, 2) + Math.Pow(locationOne.Y - locationTwo.Y, 2));
        }
        /// <summary>
        /// Finds the angle for location 1 to point to location 2. Returned in degrees.
        /// </summary>
        /// <param name="location1"></param>
        /// <param name="location2"></param>
        /// <returns>Angle in degrees from loc1 to loc2</returns>
        private float FindAngleToTarget(Vector2f location1, Vector2f location2)
        {
            return Deg((float)Math.Atan2(location1.Y - location2.Y, location1.X - location2.X)) + 180;
        }
        /// <summary>
        /// Converts radians into degrees.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns>Degrees</returns>
        private float Deg(float radians)
        {
            return (float)(radians * 180 / Math.PI);
        }
        /// <summary>
        /// Converts degrees into radians
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns>radians</returns>
        private float Rad(float degrees)
        {
            return (float)(degrees * (Math.PI / 180F));
        }
        /// <summary>
        /// Gets this robot from the player list. Requires the 'Name' variable in order to work.
        /// </summary>
        /// <param name="playerList"></param>
        /// <returns></returns>
        private IDummyPlayer GetMe(List<IDummyPlayer> playerList)
        {
            IDummyPlayer returnValue = null;
            foreach (IDummyPlayer player in playerList)
            {
                if (player.PlayerName == Name)
                {
                    returnValue = player;
                }
            }
            return returnValue;
            //return playerList.Find(player => player.PlayerName == Name);
        }
        /// <summary>
        /// Gets your base from the base list. Requires the 'Name' variable in order to work.
        /// </summary>
        /// <param name="baseList"></param>
        /// <returns></returns>
        private IDummyBase GetMeBase(List<IDummyBase> baseList)
        {
            IDummyBase returnValue = null;
            foreach (IDummyBase mybase in baseList)
            {
                if (mybase.PlayerName == Name)
                {
                    returnValue = mybase;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Gives your name to the rest of the program. Required method.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return Name;
        }
    }
}
