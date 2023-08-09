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
            return Math.Max(Math.Abs(X - other.X), Math.Abs(Y - other.Y));
        }

        public int ManhatanDistanceTo(Position other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        public override string ToString()
        {
            return Y + "" + (X + 1);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj == null) return false;
            if (!(obj is Position)) return false;
            return Equals((Position)obj);
        }

        private bool Equals(Position obj)
        {
            return Y == obj.Y && X == obj.X;
        }
        public override int GetHashCode()
        {
            return Y.GetHashCode() ^ X.GetHashCode();
        }
    }
}
