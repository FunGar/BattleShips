using BattleShips.Exceptions;
using System;
using System.Collections.Generic;
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

    public class TrackingGrid : Grid
    {
        public void PlaceShip(Ship ship, Position headPosition, Orientation orientation = Orientation.HORIZONTAL)
        {
            var tilesToChange = new List<Tile>();
            if (orientation == Orientation.HORIZONTAL)
            {
                for (var i = headPosition.X; i < headPosition.X + ship.Size; i++)
                {
                    var pos = new Position { X = i, Y = headPosition.Y };
                    if (Tiles.Where(t => t.Key.DistanceTo(pos) == 1)
                        .Any(t => t.Value.State == TileState.SHIP)
                        )
                    {
                        throw new ShipPlacementException();
                    }
                    tilesToChange.Add(Tiles[pos]);
                }
            }
            else
            {
                for (var i = headPosition.Y; i < headPosition.Y + ship.Size; i++)
                {
                    var pos = new Position { X = headPosition.X, Y = i };
                    if (Tiles.Where(t => t.Key.DistanceTo(pos) == 1)
                        .Any(t => t.Value.State == TileState.SHIP)
                        )
                    {
                        throw new ShipPlacementException();
                    }
                    tilesToChange.Add(Tiles[pos]);
                }
            }
            foreach (var tile in tilesToChange)
            {
                ship.Tiles.Add(tile);
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
    }
}
