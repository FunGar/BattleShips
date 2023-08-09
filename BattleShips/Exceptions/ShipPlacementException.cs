using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShips.Exceptions
{
    public class ShipPlacementException : Exception
    {
        public ShipPlacementException() : base("Ship cannot be placed here") { }

        public ShipPlacementException(string message) : base(message) { }
    }
}
