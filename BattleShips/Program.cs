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
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());*/

            Console.WriteLine("aaaaaaaaaa");

            Entity player1 = new Bot();
            Entity player2 = new Bot();

            player1.ArrangeShips();
            player2.ArrangeShips();

            Console.WriteLine("========================================");
            Console.WriteLine(player1.TrackingGrid);
            Console.WriteLine("========================================");
            Console.WriteLine(player2.TrackingGrid);

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

                Console.WriteLine("  1 2 3 4 5 6 7 8 9 10   |   1 2 3 4 5 6 7 8 9 10");
                for (var i = 'A'; i < 'A' + 10; i++)
                {
                    Console.Write(i + " ");
                    for (var j = 0; j < 10; j++)
                    {
                        var tile = player1.TrackingGrid.Tiles[new Position { X = j, Y = i }];
                        char charToWrite = '~';
                        switch (tile.State)
                        {
                            case TileState.WATER:
                                charToWrite = '~';
                                break;
                            case TileState.SHIP:
                                charToWrite = '■';
                                break;
                            case TileState.DESTROYED_SHIP:
                                charToWrite = '#';
                                break;
                            case TileState.MISSED_SHOT:
                                charToWrite = '*';
                                break;
                        }
                        Console.Write(charToWrite + " ");
                    }
                    Console.Write("   |   ");
                    for (var j = 0; j < 10; j++)
                    {
                        var tile = player2.TrackingGrid.Tiles[new Position { X = j, Y = i }];
                        char charToWrite = '~';
                        switch (tile.State)
                        {
                            case TileState.WATER:
                                charToWrite = '~';
                                break;
                            case TileState.SHIP:
                                charToWrite = '■';
                                break;
                            case TileState.DESTROYED_SHIP:
                                charToWrite = '#';
                                break;
                            case TileState.MISSED_SHOT:
                                charToWrite = '*';
                                break;
                        }
                        Console.Write(charToWrite + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("\n=======================\n");
            }

            if (player1.IsDefeated()) Console.WriteLine("Player 2 win");
            else Console.WriteLine("Player 1 win");
        }
    }
}
