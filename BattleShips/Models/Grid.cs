using BattleShips.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleShips.Models
{
    public class Grid
    {
        public IDictionary<Position, Tile> Tiles { get; set; } = new Dictionary<Position, Tile>();

        public Grid()
        {
            for (var i = 'A'; i < 'A' + 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    var pos = new Position { X = j, Y = i };
                    Tiles[pos] = new Tile { State = TileState.WATER };
                }
            }
        }
    }

    /// <summary>
    /// Enemy grid.
    /// A grid where player can shoot.
    /// </summary>
    public class PrimaryGrid : Grid
    {
        public PrimaryGrid()
        {
            for (var i = 'A'; i < 'A' + 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    var pos = new Position { X = j, Y = i };
                    Tiles[pos] = new Tile { State = TileState.UNDISCOVERED };
                }
            }
        }
    }

    /// <summary>
    /// Players grid.
    /// A grid where player can place ships and mark, where an enemy shoots.
    /// </summary>
    public class TrackingGrid : Grid
    {
        /// <summary>
        /// Places a ship on <see cref="TrackingGrid"/>.
        /// 
        /// </summary>
        /// <param name="ship"> a ship that needs to be placed.</param>
        /// <param name="headPosition"> position of ships head. Head is the highest tile for 
        /// <c>vertical</c> orientation or the most left tile for <c>horizontal</c> orientation.</param>
        /// <param name="orientation"> <c>vertical</c> or <c>horizontal</c>.</param>
        /// <exception cref="ShipPlacementException">Is thrown when ship cannot be placed 
        /// in provided position and orientation</exception>
        public void PlaceShip(Ship ship, Position headPosition, Orientation orientation)
        {
            var tilesToChange = new List<Tile>();
            var shipPosition = new List<Position>();
            if (orientation == Orientation.HORIZONTAL)
            {
                for (var i = headPosition.X; i < headPosition.X + ship.Size; i++)
                {
                    var pos = new Position { X = i, Y = headPosition.Y };
                    if (Tiles.Where(t => t.Key.ChebyshevDistanceTo(pos) == 1)
                        .Any(t => t.Value.State == TileState.SHIP)
                        )
                    {
                        throw new ShipPlacementException();
                    }
                    tilesToChange.Add(Tiles[pos]);
                    shipPosition.Add(pos);
                }
            }
            else
            {
                for (var i = headPosition.Y; i < headPosition.Y + ship.Size; i++)
                {
                    var pos = new Position { X = headPosition.X, Y = i };
                    if (Tiles.Where(t => t.Key.ChebyshevDistanceTo(pos) == 1)
                        .Any(t => t.Value.State == TileState.SHIP)
                        )
                    {
                        throw new ShipPlacementException();
                    }
                    tilesToChange.Add(Tiles[pos]);
                    shipPosition.Add(pos);
                }
            }
            ship.Positions = shipPosition;
            foreach (var tile in tilesToChange)
            {
                tile.State = TileState.SHIP;
            }
        }

        public override string ToString()
        {
            var toRet = "";
            for (var i = 'A'; i < 'A' + 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    toRet += (Tiles[new Position
                    {
                        Y = i,
                        X = j
                    }].State == TileState.WATER ? '~' : '■');
                }
                toRet += "\n";
            }
            return toRet;
        }

        public bool IsShipDestroyed(Ship ship)
        {
            return ship.Positions.Where(t => this.Tiles[t].State == TileState.DESTROYED_SHIP).Count() == ship.Size;
        }
    }
}
