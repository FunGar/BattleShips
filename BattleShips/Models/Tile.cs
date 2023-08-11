using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShips.Models
{
    /// <summary>
    /// States of a <see cref="Tile"/>.
    /// Descriptions:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <c>WATER</c>: a tile with water. Used on <see cref="TrackingGrid"/>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <c>Ship</c>: a tile with ship. Used on <see cref="TrackingGrid"/>.
    /// </description>
    /// </item>
    /// <item>
    /// <description><c>UNDISCOVERED</c>: an undiscovered enemy tile. 
    /// Used in <see cref="PrimaryGrid"/> to mask enemy ships.</description>
    /// </item>
    /// <item>
    /// <description><c>DESTROYED_SHIP</c>: a tile with destroyed ship.
    /// Used on both <see cref="PrimaryGrid"/> and <see cref="TrackingGrid"/></description>
    /// </item>
    /// <item>
    /// <description><c>MISSED_SHOT</c>: a tile without ship where player shoot.
    /// Used on both <see cref="PrimaryGrid"/> and <see cref="TrackingGrid"/></description>
    /// </item>
    /// <item>
    /// <description>
    /// <c>WATER_WITH_SHIP_FRAGMENTS</c>: a tile adjucent to destroyed ship.
    /// Is used to show ship fragments that could damage other ships if they were nearby.
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    public enum TileState
    {
        WATER, SHIP, UNDISCOVERED, DESTROYED_SHIP, MISSED_SHOT, WATER_WITH_SHIP_FRAGMENTS
    }
    /// <summary>
    /// Just a tile with a state.
    /// </summary>
    public class Tile
    {
        public TileState State { get; set; } = TileState.UNDISCOVERED;
    }
}
