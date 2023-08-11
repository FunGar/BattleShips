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
        public static double speed = 0.5;
        public static string winMessage = null;
        public static Entity player1 = new Bot();
        public static Entity player2 = new Bot();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            player1.ArrangeShips();
            player2.ArrangeShips();
            var form = new BattleshipsForm();
            Task.Factory.StartNew(() => { Application.Run(form); });
        

            var whoseTurn = 1;

            while (!player1.IsDefeated() && !player2.IsDefeated())
            {
                Console.WriteLine($"Player {whoseTurn} turn");
                if (whoseTurn == 1)
                {
                    var pos = player1.Shoot();
                    Console.WriteLine(pos);
                    var responce = player2.GetShot(pos);
                    Console.WriteLine(responce);
                    player1.GetResponse(responce);

                    if (responce == ResponseToShot.WATER)
                    {
                        whoseTurn = 2;
                    }
                }
                else
                {
                    var pos = player2.Shoot();
                    Console.WriteLine(pos);
                    var responce = player1.GetShot(pos);
                    Console.WriteLine(responce);
                    player2.GetResponse(responce);

                    if (responce == ResponseToShot.WATER)
                    {
                        whoseTurn = 1;
                    }
                }
                while(speed == 0)
                {
                    Task.Delay(100).Wait();
                }
                Task.Delay((int)(1000/speed)).Wait();
            }

            if (player1.IsDefeated()) winMessage = "Player 2 win!";
            else winMessage = "Player 1 win!";

            while (!form.IsDisposed)
            {
                Task.Delay(500).Wait();
            }
        }
    }
    
}
