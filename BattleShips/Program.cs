using BattleShips.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleShips
{
    internal static class Program
    {
        public static double Speed = 1;
        public static string WinMessage = null;
        public static Entity Player1 = new Bot();
        public static Entity Player2 = new Bot();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            Player1.ArrangeShips();
            Player2.ArrangeShips();
            var form = new BattleshipsForm();
            Task.Factory.StartNew(() => { Application.Run(form); });
        

            var whoseTurn = 1;

            Task.Delay(1000).Wait();

            while (!Player1.IsDefeated() && !Player2.IsDefeated())
            {
                if (whoseTurn == 1)
                {
                    var pos = Player1.Shoot();
                    var responce = Player2.GetShot(pos);
                    Player1.GetResponse(responce);

                    if (responce == ResponseToShot.WATER)
                    {
                        whoseTurn = 2;
                    }
                }
                else
                {
                    var pos = Player2.Shoot();
                    var responce = Player1.GetShot(pos);
                    Player2.GetResponse(responce);

                    if (responce == ResponseToShot.WATER)
                    {
                        whoseTurn = 1;
                    }
                }
                while(Speed == 0)
                {
                    Task.Delay(100).Wait();
                }
                Task.Delay((int)(1000/Speed)).Wait();
            }

            if (Player1.IsDefeated()) WinMessage = "Player 2 win!";
            else WinMessage = "Player 1 win!";

            while (!form.IsDisposed)
            {
                Task.Delay(500).Wait();
            }
        }
    }
    
}
