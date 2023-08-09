using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShips.Models
{
    public enum TileState
    {
        WATER, SHIP, UNDISCOVERED, DESTROYED_SHIP, MISSED_SHOT
    }

    public class Tile
    {
        public TileState State { get; set; } = TileState.UNDISCOVERED;
    }
}
