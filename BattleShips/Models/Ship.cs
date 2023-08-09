using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShips.Models
{
    public enum Orientation
    {
        HORIZONTAL, VERTICAL
    }

    public class Ship
    {
        public virtual int Size { get; set; }
        public List<Tile> Tiles { get; set; } = new List<Tile>();
        public bool IsDestroyed()
        {
            return Tiles.Where(t => t.State == TileState.DESTROYED_SHIP).Count() == Size;
        }
    }
}
