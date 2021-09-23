using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace DropParty
{
    public static class Debug
    {
        private static Text TextToDisplay;
        private static Dictionary<string, string> myDictionary;
        private static List<TimeKeeper> timeKeeper;
        private static Clock Timer = new Clock();
        private static int UpdateCounter;
        private static Vector2f Position = new Vector2f(1200, 0);

        static Debug()
        {
            TextToDisplay = new Text("", new Font(@"Resources/sansation.ttf"), 18)
            {
                FillColor = new Color(255, 255, 255, 126)
            };
            TextToDisplay.Position += new Vector2f(0, 0);
            myDictionary = new Dictionary<string, string>();
            timeKeeper = new List<TimeKeeper>();
        }

        public static void Update()
        {
            if (Timer.ElapsedTime.AsSeconds() > UpdateCounter + 1)
            {
                UpdateCounter++;

                foreach (var keeper in timeKeeper)
                {
                    keeper.RunsPerSecondDisplay = keeper.RunsPerSecondCounter;
                    keeper.RunsPerSecondCounter = 0;
                }
            }
        }

        private static void UpdateText()
        {
            TextToDisplay.DisplayedString = string.Join("\n", myDictionary) + "\n" + string.Join("\n", timeKeeper);
        }

        private static void CheckKeyAdd(string key)
        {
            if (!myDictionary.ContainsKey(key))
            {
                myDictionary[key] = "0";
            }
        }
        /// <summary>
        /// Shows a value on screen.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Assign(string key, int value)
        {    
            myDictionary[key] = value.ToString();

            UpdateText();
        }

        public static void Assign(string key, object value)
        {
            myDictionary[key] = value.ToString();

            UpdateText();
        }

        public static void Assign(string key, double value)
        {
            myDictionary[key] = value.ToString();

            UpdateText();
        }

        public static void Assign(string key, string value)
        {
            myDictionary[key] = value;

            UpdateText();
        }

        public static void Assign(string key, bool value)
        {
            myDictionary[key] = value.ToString();

            UpdateText();
        }
        /// <summary>
        /// Can add to a number value stored in the string
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddTo(string key, double value)
        {
            CheckKeyAdd(key);

            myDictionary[key] = (Convert.ToDouble(myDictionary[key]) + value).ToString();

            UpdateText();
        }

        public static void Lap(string value)
        {
            Assign(value, Timer.ElapsedTime.AsMilliseconds());
        }


        /// <summary>
        /// Adds a timekeeper named with the inputted value, or records a lap.
        /// </summary>
        /// <param name="value"></param>
        public static void Time(string value)
        {
            if (!timeKeeper.Exists(x => x.Name == value))
            {
                timeKeeper.Add(new TimeKeeper(value));
            }

            timeKeeper.Where(x => x.Name == value).First().Time(Timer.ElapsedTime.AsMilliseconds());

            UpdateText();
        }

        public static void Draw(RenderTarget renderTarget, RenderStates renderStates)
        {
            renderStates.Transform.Translate(Position);
            TextToDisplay.Draw(renderTarget, renderStates);
        }

        private class TimeKeeper
        {
            public string Name;
            private int AverageTime;
            private int TotalTime;
            private int TimesRan;
            private bool Started;
            private int startTime;
            private int LastTime;
            private int LargestTime;
            public int RunsPerSecondCounter;
            public int RunsPerSecondDisplay;

            public TimeKeeper(string name)
            {
                Name = name;
            }

            public void Time(int time)
            {
                if (Started)
                {
                    LastTime = time - startTime;
                    TotalTime += LastTime;
                    TimesRan++;
                    RunsPerSecondCounter++;
                    AverageTime = TotalTime / TimesRan;
                    if (LargestTime < LastTime)
                    {
                        LargestTime = LastTime;
                    }
                }
                else
                {
                    startTime = time;
                }

                Started = !Started;
            }

            public override string ToString()
            {
                return string.Format("{0}: Avg {1}, Total {2}, Runs {3}, LstTme {4}, LgTme {5}, RPS {6}", Name, AverageTime, TotalTime, TimesRan, LastTime, LargestTime, RunsPerSecondDisplay);
            }
        }
    }
}

