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
        public List<Position> Positions { get; set; } = new List<Position>();
    }
}
