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
    public class GameLoop
    {
        public static RenderWindow myWindow;
        Map myMap;
        bool isPaused = false;
        public GameLoop()
        {
            ContextSettings contextSettings = new ContextSettings
            {
                AntialiasingLevel = 8
            };

            myWindow = new RenderWindow(VideoMode.DesktopMode, "Drop Party", Styles.Fullscreen, contextSettings);
            myWindow.SetVerticalSyncEnabled(true);
            myWindow.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            myWindow.Closed += new EventHandler(OnClose);

            List<Type> playerTypeList = new List<Type>() 
            {
                typeof(ExampleBot),
                typeof(ExampleBot),
                typeof(ExampleBot),
                typeof(ExampleBot),
            };


            myMap = new Map(playerTypeList);
        }

        public void Run()
        {
            while (myWindow.IsOpen) //GAME LOOP
            {
                myWindow.DispatchEvents();
                //Update
                if (!isPaused)
                {
                    myMap.Update();
                    Debug.Update();
                }
                //Draw
                myWindow.Clear();
                myMap.Draw(myWindow, RenderStates.Default);
                Debug.Draw(myWindow, RenderStates.Default);
                myWindow.Display();
            }
        }
        /// <summary>
        ///  Runs when the window closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClose(object sender, EventArgs e)
        {
            myWindow.Close();
        }
        /// <summary>
        /// Pauses program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPause(object sender, EventArgs e)
        {
            isPaused = !isPaused;
        }

        /// <summary>
        /// Runs when the user presses a key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnKeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Escape:
                    OnClose(sender, e);
                    break;

                case Keyboard.Key.Space:
                    OnPause(sender, e);
                    break;
            }
        }
    }

}
