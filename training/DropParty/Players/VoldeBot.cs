using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using neural_network;

namespace DropParty
{
    public class VoldeBot : IControllable
    {
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

        // =================================================================================================================
        // START HERE
        // =================================================================================================================
        private const float FLOAT_NULL = 0;

        DatasetWrapper dataset = null; // replace with actual data

        private string Name;
        private bool uncalibratedOffset = true;
        private float offsetAngle;

        private Vector2f lastPosition;
        private Vector2f homePosition;

        // constructor
        public VoldeBot(int myNum)
        {
            Name = "VoldeBot" + myNum.ToString();
        }



        private float[] filterInput(List<IDummyCoin> coinList, List<IDummyPlayer> playerList, List<IDummyBullet> bulletList, List<IDummyBase> baseList, int mapRadius)
        {
            float[] input = new float[72];
            byte index = 0;

            IDummyPlayer me = GetMe(playerList);

            // track distance to home base using distance (even) and direction (odd); range 0-1
            IDummyBase meBase = GetMeBase(baseList);
            input[index] = GetPythag(me.Position, meBase.Position);
            index++;
            input[index] = FindAngleToTarget(me.Position, meBase.Position);
            index++;

            // track opponent location using distance (even) and direction (odd) and is ordered by proximity; range 2-13
            List<IDummyPlayer> opponentList = new List<IDummyPlayer>();
            foreach (IDummyPlayer opponent in playerList)
            {
                if (opponent != me)
                {
                    opponentList.Add(opponent);
                }
            }
            opponentList = opponentList.OrderBy(opponent => GetPythag(me.Position, opponent.Position)).ToList();
            foreach (IDummyPlayer opponent in opponentList)
            {
                input[index] = GetPythag(me.Position, opponent.Position);
                index++;
                input[index] = FindAngleToTarget(me.Position, opponent.Position);
                index++;
            }
            while (index < 14)
            {
                input[index] = FLOAT_NULL;
                index++;
            }

            // track 16 closest coins using distance (mod 3 remainder 2), direction (mod 3 remainder 0), and coin value (mod 3 remainder 1); range 14-61
            coinList.RemoveAll(coin => coin.isHeld);
            if (coinList.Count > 0)
            {
                //Finds closest coin
                coinList = coinList.OrderBy(coin => GetPythag(coin.Position, me.Position)).ToList<IDummyCoin>();
                for (int i = 0; i < 16 && i < coinList.Count; i++)
                {
                    input[index] = GetPythag(me.Position, coinList[i].Position);
                    index++;
                    input[index] = FindAngleToTarget(me.Position, coinList[i].Position);
                    index++;
                    input[index] = coinList[i].Value;
                    index++;
                }
            }
            while (index < 62)
            {
                input[index] = FLOAT_NULL;
                index++;
            }

            // track 5 closest bullets using distance (even), and direction (odd); range 62-71
            if (bulletList.Count > 0)
            {
                //Finds closest bullet
                bulletList = bulletList.OrderBy(bullet => GetPythag(bullet.Position, me.Position)).ToList<IDummyBullet>();
                for (int i = 0; i < 5 && i < bulletList.Count; i++)
                {
                    input[index] = GetPythag(me.Position, bulletList[i].Position);
                    index++;
                    input[index] = FindAngleToTarget(me.Position, bulletList[i].Position);
                    index++;
                }
            }
            while (index < 72)
            {
                input[index] = FLOAT_NULL;
                index++;
            }

            return input;
        }

        public float GetFire(List<IDummyCoin> coinList, List<IDummyPlayer> playerList, List<IDummyBullet> bulletList, List<IDummyBase> baseList, int mapRadius)
        {
            float value = dataset.Get(filterInput(coinList, playerList, bulletList, baseList, mapRadius))[1];
            if (value + offsetAngle < 0)
            {
                return -1;
            }
            else
            {
                return value + offsetAngle;
            }
        }

        public float GetMovement(List<IDummyCoin> coinList, List<IDummyPlayer> playerList, List<IDummyBullet> bulletList, List<IDummyBase> baseList, int mapRadius)
        {
            IDummyPlayer me = GetMe(playerList);
            if (uncalibratedOffset) // first-time call only
            {
                Vector2f center = new Vector2f(0, 0);
                foreach (IDummyBase bases in baseList)
                {
                    center.X += bases.Position.X;
                    center.Y += bases.Position.Y;
                }
                center.X /= baseList.Count;
                center.Y /= baseList.Count;
                uncalibratedOffset = false;

                offsetAngle = FindAngleToTarget(me.Position, center);
                homePosition = GetMeBase(baseList).Position;
            }

            lastPosition = me.Position;

            float value = offsetAngle + 360f*dataset.Get(filterInput(coinList, playerList, bulletList, baseList, mapRadius))[0];
            return value % 360;
        }

        public Vector2f GetLastPosition()
        {
            return lastPosition;
        }

        public Vector2f GetHomePosition()
        {
            return homePosition;
        }

        // =================================================================================================================
        // For training VoldeBot
        // =================================================================================================================

        // for overwriting the given dataset
        public void SetDataset(DatasetWrapper dataset)
        {
            this.dataset = dataset;
        }

        // for sending the given dataset to the training program
        public DatasetWrapper GetDataset()
        {
            return dataset;
        }

        // mutates the associated dataset
        public void Mutate(float range)
        {
            dataset.Mutate(range);
        }
    }
}
