using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using neural_network;

namespace DropParty
{
    public static class TrainingProgram
    {
        private static string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\dat\";
        private static string weightFileSuffix = "Weight.csv";
        private static string biasFileSuffix = "Bias.csv";

        private const UInt16 gens = 50; // number of generations
        private const Int32 iters = 25; // number of trials; only last trial is shown
        private const float range = 0.005f; // how much mutations there are; higher means more
        private const byte studentCount = 7; // number of VoldeBots in training
        private const byte gradCount = 4; //number of VoldeBots allowed to move to next trial
        private const Int32 timeOut = 900; // number of cycles before timeout (1000 cycles = 16sec)
        private const float collectivePassingScore = 0.03f; // score for rewards to be counted

        private const Int32 bigDistanceInterval = 75; // frequency of bigDistance updates

        private const float distanceMultiplier = 0.001f; // moving bots are better than stationary bots
        private const float bigDistanceToDistanceRatio = 10f; // encourage efficient moving bots
        private const float generationalDistanceRatio = 0.01f; // encourage exploration

        private const float coinMultiplier = 1f; // bot that collects coins are better than bots that don't

        private const float pkMultiplier = 0f; // 0.1f; // encourage killing

        private const float botTotal = 7; // total number of bots

        class PlayerMetrics
        {
            public Vector2f lastPosition { get; set; }
            public float distanceTravelled { get; set; } // parameter that resets on death to discourage death
            public float bigDistanceTravelled { get; set; } // parameter that discourages shuffling back and forth
            public float generationalDistanceTravelled { get; set; } // parameter that does not reset on death
        }
        struct Student
        {
            public DropParty.VoldeBot voldeBot;
            public Player associatedPlayer;
            public PlayerMetrics metrics;
        }
        private static int[] rewards = new int[gradCount] {2, 2, 0, 0}; // new int[2] {3, 1}; // the file for the winning first place bot will replace 3 positions; the file for the second place bot will replace 1 position

        // gets distance between two locations
        private static float GetPythag(Vector2f locationOne, Vector2f locationTwo)
        {
            return (float)Math.Sqrt(Math.Pow(locationOne.X - locationTwo.X, 2) + Math.Pow(locationOne.Y - locationTwo.Y, 2));
        }

        public static void Main(string[] args)
        {
            List<DatasetWrapper> datasets = new List<DatasetWrapper>();
            List<Type> typesList = new List<Type>();
            for (int i=0; i<botTotal - studentCount; i++)
            {
                typesList.Add(typeof(DropParty.ExampleBot));
            }
            for (int i=0; i<studentCount; i++)
            {
                typesList.Add(typeof(DropParty.VoldeBot));
            }

            GameLoop gameLoop = new GameLoop();
            for (UInt16 generationNum = 1; generationNum <= gens; generationNum++)
            {
                // Load data file from disk - constructor
                for (int n = 0; n < studentCount; n++)
                {
                    string filePrefix = "Behavior"+n.ToString();
                    datasets.Add(new DatasetWrapper(path + filePrefix + weightFileSuffix, path + filePrefix + biasFileSuffix));
                }

                for (Int32 i = 0; i < iters; ++i)
                {
                    List<Student> students = new List<Student>();
                    CollisionHandler.Clear();
                    gameLoop.Restart(typesList);

                    int m = 0;
                    int[] counter = new int[gradCount];
                    foreach (Player player in gameLoop.getMap().GetPlayerHandler.GetPlayers)
                    {
                        if (player.Controllable.GetType() == typeof(DropParty.VoldeBot))
                        {
                            Student temp = new Student();
                            temp.voldeBot = (DropParty.VoldeBot) player.Controllable;
                            temp.associatedPlayer = player;
                            temp.metrics = new PlayerMetrics();

                            temp.voldeBot.SetDataset(datasets[m]);
                            if (counter[m] != 0)
                            {
                                temp.voldeBot.Mutate(range);  // Mutates bot
                            }
                            students.Add(temp);

                            if (counter[m] > rewards[m])
                            {
                                m++;
                            }
                            counter[m]++;
                        }
                    }

                    // Setup simulation - inputs, weights, from the data set
                    bool flag = false;
                    int cycle = 0;
                    float generationalDistance = 0;
                    float bigDistance = 0;
                    do
                    {
                        // Compute simulation
                        flag = gameLoop.Step(i+1 == iters);

                        // update distances
                        for(int n = 0; n < students.Count; n++)
                        {
                            float distance = TrainingProgram.GetPythag(students[n].voldeBot.GetLastPosition(), students[n].metrics.lastPosition);
                            bigDistance = bigDistance + distance;
                            generationalDistance = generationalDistance + distance;
                        
                            if (distance > students[n].associatedPlayer.GetPlayerSpeed() * 1.2f) // player was teleported indicating player death, which triggers reset of player stats
                            {
                                students[n].metrics.lastPosition = students[n].voldeBot.GetHomePosition();
                                students[n].metrics.distanceTravelled = 0;

                                bigDistance = 0;
                                students[n].metrics.bigDistanceTravelled = 0;
                            }
                            else
                            {
                                students[n].metrics.lastPosition = students[n].voldeBot.GetLastPosition();
                                students[n].metrics.distanceTravelled = students[n].metrics.distanceTravelled + distance;
                            }
                        
                            if (i % 75 == 0)
                            {
                                students[n].metrics.bigDistanceTravelled = students[n].metrics.bigDistanceTravelled + bigDistance;
                                bigDistance = 0;
                            }
                        }
                        cycle++;
                        if (cycle >= timeOut) break; // (gameLoop.getMap().getFreeCoinCount() <= 0 || cycle >= timeOut) break;
                    }
                    while(flag);

                    // Analyze simulation - filter, transform, to the data set
                    int p = gameLoop.getMap().getFreeCoinCount();
                    int q = gameLoop.getMap().getTotalCoinCount();
                    float collectiveScore = 1-((float)gameLoop.getMap().getFreeCoinCount())/gameLoop.getMap().getTotalCoinCount();

                    // scoring rubric
                    students = students.OrderBy(student =>
                        coinMultiplier*(student.associatedPlayer.HeldValue+student.associatedPlayer.BankedValue)
                        + distanceMultiplier*(student.metrics.distanceTravelled+bigDistanceToDistanceRatio*student.metrics.bigDistanceTravelled+generationalDistanceRatio*student.metrics.generationalDistanceTravelled)/(1+bigDistanceToDistanceRatio+generationalDistanceRatio)
                        + pkMultiplier*(student.associatedPlayer.playerKills)
                        ).ToList();
                    
                    datasets.Clear();
                    if (generationNum != gens)
                    {
                        int total = students.Count;
                        Student[] graduater = new Student[gradCount];
                        int k = 0;

                        while (total - gradCount < students.Count)
                        {
                            int index = students.Count-1;
                            datasets.Add(students[index].voldeBot.GetDataset());
                            students.RemoveAt(index);
                            k++;
                        }
                        // students array contains only those left behind at this point
                    }
                    else
                    {
                        foreach (Student student in students)
                        {
                            datasets.Add(student.voldeBot.GetDataset());
                        }
                    }
                }
            }
            for (int n = 0; n < studentCount; n++)
            {
                string filePrefix = "Behavior"+n.ToString();
                datasets[n].ToFile(path + filePrefix + weightFileSuffix, path + filePrefix + biasFileSuffix); // output to file
            }
        }
    }
}