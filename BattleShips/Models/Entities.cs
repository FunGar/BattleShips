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

    public abstract class Entity
    {
        protected readonly Dictionary<Tile, Ship> _ships = new Dictionary<Tile, Ship>();
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
        public abstract void GetResponse(ResponseToShot response);
        public ResponseToShot GetShot(Position shotPosition)
        {
            switch (TrackingGrid.Tiles[shotPosition].State)
            {
                case TileState.WATER:
                    TrackingGrid.Tiles[shotPosition].State = TileState.MISSED_SHOT;
                    break;
                case TileState.SHIP:
                    TrackingGrid.Tiles[shotPosition].State = TileState.DESTROYED_SHIP;
                    if (_ships[TrackingGrid.Tiles[shotPosition]].IsDestroyed())
                    {
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
            return !_ships.Values.Where(s => !s.IsDestroyed()).Any();
        }
    }

    public class Bot : Entity
    {

        private enum ShootingDirection
        {
            UP, LEFT, RIGHT, DOWN, NONE
        }

        private static readonly Random _random = new Random();

        private readonly List<Position> _hitPositions = new List<Position>();

        private ShootingDirection direction = ShootingDirection.NONE;
        private Position lastShotPosition;

        public Bot() : base() { }

        public override Position Shoot()
        {
            Console.WriteLine("-----");
            _hitPositions.ForEach(Console.WriteLine);
            Console.WriteLine("-----");
            if (_hitPositions.Count == 0)
            {
                lastShotPosition = _undiscoveredTiles[_random.Next(_undiscoveredTiles.Count)];
                return lastShotPosition;
            }
            else if (_hitPositions.Count == 1)
            {
                var possibleShots = _undiscoveredTiles
                    .Where(a => a.ManhatanDistanceTo(_hitPositions[0]) == 1)
                    .ToList();
                Console.WriteLine("=====");
                possibleShots.ForEach(Console.WriteLine);
                Console.WriteLine("=====");
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
                EnemyGrid.Tiles[lastShotPosition].State = TileState.DESTROYED_SHIP;

                foreach (var hitPos in _hitPositions)
                {
                    _undiscoveredTiles
                        .Where(t => t.DistanceTo(hitPos) == 1 && EnemyGrid.Tiles[t].State == TileState.UNDISCOVERED)
                        .ToList()
                        .ForEach(t => EnemyGrid.Tiles[t].State = TileState.WATER);
                    _undiscoveredTiles
                        .RemoveAll(t => t.DistanceTo(hitPos) <= 1);
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

        public override void ArrangeShips()
        {
            var ships = new List<Ship>();
            for (var i = 1; i <= 4; i++)
            {
                for (var j = 4; j >= i; j--)
                {
                    ships.Add(new Ship { Size = i });
                }
            }
            ships.OrderByDescending(s => s.Size).ToList().ForEach(ship =>
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
                        ship.Tiles.ForEach(tile => this._ships[tile] = ship);
                    }
                    catch (ShipPlacementException) { }
                }
            });
        }
    }
}
