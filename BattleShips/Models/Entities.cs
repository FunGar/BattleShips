using BattleShips.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShips.Models
{
    public enum ResponseToShot
    {
        WATER, HIT, DESTROYED
    }

    /// <summary>
    /// Entity that can play the game.
    /// Currently there is only <see cref="Bot"/>, but this can change thanks to <c>Entity</c> class.
    /// </summary>
    public abstract class Entity
    {
        protected readonly Dictionary<Position, Ship> _ships = new Dictionary<Position, Ship>();
        protected readonly List<Position> _undiscoveredTiles = new List<Position>();

        protected Entity()
        {
            for (var i = 'A'; i < 'A' + 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    _undiscoveredTiles.Add(new Position { Y = i, X = j });
                }
            }
        }

        public TrackingGrid TrackingGrid { get; set; } = new TrackingGrid();
        public PrimaryGrid EnemyGrid { get; set; } = new PrimaryGrid();
        public abstract void ArrangeShips();
        public abstract Position Shoot();

        /// <summary>
        /// Is trigered to inform an <see cref="Entity"/> about last shot.
        /// Changes <see cref="TileState"/> of <see cref="Tile"/> on <see cref="EnemyGrid"/>.
        /// </summary>
        /// <param name="response"></param>
        public abstract void GetResponse(ResponseToShot response);

        /// <summary>
        /// Is triggered when an enemy shoots.
        /// </summary>
        /// <param name="shotPosition"> a position where an enemy shot.</param>
        /// <returns><list type="bullet">
        /// <item><c>MISSED_SHOT</c> if an enemy shot into the water.</item>
        /// <item><c>HIT</c> if an enemy shot into the ship.</item>
        /// <item><c>DESTROYED_SHIP</c> if an enemy shot into the ship and destroyed it..</item>
        /// </list></returns>
        public ResponseToShot GetShot(Position shotPosition)
        {
            switch (TrackingGrid.Tiles[shotPosition].State)
            {
                case TileState.WATER:
                    TrackingGrid.Tiles[shotPosition].State = TileState.MISSED_SHOT;
                    break;
                case TileState.SHIP:
                    TrackingGrid.Tiles[shotPosition].State = TileState.DESTROYED_SHIP;
                    if (TrackingGrid.IsShipDestroyed(_ships[shotPosition]))
                    {
                        foreach(var shipPosition in _ships[shotPosition].Positions)
                        {
                            TrackingGrid.Tiles
                                .Where(t => t.Key.ChebyshevDistanceTo(shipPosition) == 1 && t.Value.State == TileState.WATER)
                                .ToList()
                                .ForEach(t => t.Value.State = TileState.WATER_WITH_SHIP_FRAGMENTS);
                        }
                        return ResponseToShot.DESTROYED;
                    }
                    else
                    {
                        return ResponseToShot.HIT;
                    }
            }
            return ResponseToShot.WATER;
        }
        public bool IsDefeated()
        {
            return !_ships.Values.Where(s => !TrackingGrid.IsShipDestroyed(s)).Any();
        }
    }

    public class Bot : Entity
    {

        private enum ShootingDirection
        {
            UP, LEFT, RIGHT, DOWN, NONE
        }

        private static readonly Random _random = new Random();

        /// <summary>
        /// List of last positions where enemy ship got hit.
        /// After the ship is destroyed the list is creared.
        /// </summary>
        private readonly List<Position> _hitPositions = new List<Position>();

        private ShootingDirection direction = ShootingDirection.NONE;
        private Position lastShotPosition;

        public Bot() : base() { }

        /// <summary>
        /// Shoots the enemy grid.
        /// Bot shoots according to the tactic:
        /// <list type="bullet">
        /// <item>
        /// If <see cref="_hitPositions"/> is empty -> shoots randomly.
        /// </item>
        /// <item>
        /// If <see cref="_hitPositions"/> have 1 element -> shoots randomly in adjucent undiscovered tiles.
        /// </item>
        /// <item>
        /// If <see cref="_hitPositions"/> have 2 or more elements -> remembers <see cref="direction"/> and shoot in this direction.
        /// After first miss or when gets to the end of the map -> changes 
        /// direction to oposite and shoots next undiscovered tile in this direction.
        /// </item>
        /// </list>
        /// </summary>
        /// <returns>position where bot shoots.</returns>
        public override Position Shoot()
        {
            if (_hitPositions.Count == 0)
            {
                lastShotPosition = _undiscoveredTiles[_random.Next(_undiscoveredTiles.Count)];
                return lastShotPosition;
            }
            else if (_hitPositions.Count == 1)
            {
                var possibleShots = _undiscoveredTiles
                    .Where(a => a.ManhatanDistanceTo(_hitPositions[0]) == 1)
                    .ToList();;
                lastShotPosition = possibleShots[_random.Next(possibleShots.Count)];
                return lastShotPosition;
            }
            else
            {
                if (direction == ShootingDirection.LEFT)
                {
                    lastShotPosition = new Position { X = _hitPositions.Last().X - 1, Y = _hitPositions.Last().Y };
                    if (lastShotPosition.X == -1 ||
                        (EnemyGrid.Tiles[lastShotPosition].State != TileState.UNDISCOVERED &&
                        EnemyGrid.Tiles[lastShotPosition].State != TileState.DESTROYED_SHIP))
                    {
                        direction = ShootingDirection.RIGHT;
                        return Shoot();
                    }
                    while (EnemyGrid.Tiles[lastShotPosition].State != TileState.UNDISCOVERED)
                    {
                        lastShotPosition.X--;
                    }
                    return lastShotPosition;
                }
                else if (direction == ShootingDirection.RIGHT)
                {
                    lastShotPosition = new Position { X = _hitPositions.Last().X + 1, Y = _hitPositions.Last().Y };
                    if (lastShotPosition.X == 10 ||
                        (EnemyGrid.Tiles[lastShotPosition].State != TileState.UNDISCOVERED &&
                        EnemyGrid.Tiles[lastShotPosition].State != TileState.DESTROYED_SHIP))
                    {
                        direction = ShootingDirection.LEFT;
                        return Shoot();
                    }
                    while (EnemyGrid.Tiles[lastShotPosition].State != TileState.UNDISCOVERED)
                    {
                        lastShotPosition.X++;
                    }
                    return lastShotPosition;
                }
                else if (direction == ShootingDirection.UP)
                {
                    lastShotPosition = new Position { X = _hitPositions.Last().X, Y = (char)(_hitPositions.Last().Y - 1) };
                    if (lastShotPosition.Y == 'A' - 1 ||
                        (EnemyGrid.Tiles[lastShotPosition].State != TileState.UNDISCOVERED &&
                        EnemyGrid.Tiles[lastShotPosition].State != TileState.DESTROYED_SHIP))
                    {
                        direction = ShootingDirection.DOWN;
                        return Shoot();
                    }
                    while (EnemyGrid.Tiles[lastShotPosition].State != TileState.UNDISCOVERED)
                    {
                        lastShotPosition.Y--;
                    }
                    return lastShotPosition;
                }
                else
                {
                    lastShotPosition = new Position { X = _hitPositions.Last().X, Y = (char)(_hitPositions.Last().Y + 1) };
                    if (lastShotPosition.Y == 'K' ||
                        (EnemyGrid.Tiles[lastShotPosition].State != TileState.UNDISCOVERED &&
                        EnemyGrid.Tiles[lastShotPosition].State != TileState.DESTROYED_SHIP))
                    {
                        direction = ShootingDirection.UP;
                        return Shoot();
                    }
                    while (EnemyGrid.Tiles[lastShotPosition].State != TileState.UNDISCOVERED)
                    {
                        lastShotPosition.Y++;
                    }
                    return lastShotPosition;
                }
            }
        }

        public override void GetResponse(ResponseToShot response)
        {
            if (response == ResponseToShot.DESTROYED)
            {
                _hitPositions.Add(lastShotPosition);
                EnemyGrid.Tiles[lastShotPosition].State = TileState.DESTROYED_SHIP;

                foreach (var hitPos in _hitPositions)
                {// TODO: change trackingBoard upon destruction aroun ship
                    _undiscoveredTiles
                        .Where(t => t.ChebyshevDistanceTo(hitPos) == 1 && EnemyGrid.Tiles[t].State == TileState.UNDISCOVERED)
                        .ToList()
                        .ForEach(t => EnemyGrid.Tiles[t].State = TileState.WATER_WITH_SHIP_FRAGMENTS);
                    _undiscoveredTiles
                        .RemoveAll(t =>  t.ChebyshevDistanceTo(hitPos) <= 1);
                }

                _hitPositions.Clear();
                direction = ShootingDirection.NONE;
            }
            else if (response == ResponseToShot.HIT)
            {
                _undiscoveredTiles.Remove(lastShotPosition);
                EnemyGrid.Tiles[lastShotPosition].State = TileState.DESTROYED_SHIP;
                if (_hitPositions.Count == 1)
                {
                    if (_hitPositions[0].X > lastShotPosition.X)
                    {
                        direction = ShootingDirection.LEFT;
                    }
                    else if (_hitPositions[0].X < lastShotPosition.X)
                    {
                        direction = ShootingDirection.RIGHT;
                    }
                    else if (_hitPositions[0].Y > lastShotPosition.Y)
                    {
                        direction = ShootingDirection.UP;
                    }
                    else
                    {
                        direction = ShootingDirection.DOWN;
                    }
                }
                _hitPositions.Add(lastShotPosition);
            }
            else
            {
                _undiscoveredTiles.Remove(lastShotPosition);
                EnemyGrid.Tiles[lastShotPosition].State = TileState.MISSED_SHOT;
                if (_hitPositions.Count > 1)
                {
                    direction = 3 - direction;
                }
            }
        }

        /// <summary>
        /// Randomly arranges ships for a bot.
        /// </summary>
        public override void ArrangeShips()
        {
            var shipsToArrange = new List<Ship>();
            for (var i = 1; i <= 4; i++)
            {
                for (var j = 4; j >= i; j--)
                {
                    shipsToArrange.Add(new Ship { Size = i });
                }
            }
            shipsToArrange.OrderByDescending(s => s.Size).ToList().ForEach(ship =>
            {
                bool needToPlace = true;
                while (needToPlace)
                {
                    try
                    {
                        var maxX = 10;
                        var maxY = 10;
                        var orientation = _random.Next(2) == 0 ? Orientation.HORIZONTAL : Orientation.VERTICAL;

                        if (orientation == Orientation.HORIZONTAL)
                        {
                            maxX = 11 - ship.Size;
                        }
                        else
                        {
                            maxY = 11 - ship.Size;
                        }

                        var pos = new Position
                        {
                            X = _random.Next(maxX),
                            Y = (char)('A' + _random.Next(maxY))
                        };
                        TrackingGrid.PlaceShip(
                            ship,
                            pos,
                            orientation
                            );
                        needToPlace = false;
                        ship.Positions.ForEach(tile => this._ships[tile] = ship);
                    }
                    catch (ShipPlacementException) { }
                }
            });
        }
    }
}
