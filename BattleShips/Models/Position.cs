using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShips.Models
{
    public class Position
    {
        public char Y { get; set; }
        public int X { get; set; }

        public int DistanceTo(Position other)
        {
            return Math.Max(Math.Abs(this.X - other.X), Math.Abs(this.Y - other.Y));
        }
    }
}
