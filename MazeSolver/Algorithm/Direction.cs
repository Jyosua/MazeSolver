using System.Drawing;

namespace MazeSolver.Algorithm
{
    /// <summary>
    /// Enum representing neighboring pixel directions in order of lowest x, y to highest
    /// </summary>
    public enum Direction
    {
        Northwest,
        North,
        Northeast,
        West,
        East,
        Southeast,
        South,
        Southwest,
    }

    public static class Directions
    {
        private static readonly Dictionary<Direction, Point> Offsets = new()
        {
            { Direction.Northwest, new Point(-1, -1) },
            { Direction.North, new Point(0, -1) },
            { Direction.Northeast, new Point(1, -1) },
            { Direction.West, new Point(-1, 0) },
            { Direction.East, new Point(1, 0) },
            { Direction.Southeast, new Point(1, 1) },
            { Direction.South, new Point(0, 1) },
            { Direction.Southwest, new Point(-1, 1) }
        };

        /// <summary>
        /// Returns a Point representing the neighbor's positional offset given the direction from the node
        /// </summary>
        /// <param name="direction">Orientational direction from the original node</param>
        public static Point GetOffset(this Direction direction) => Offsets[direction];

        /// <summary>
        /// A performant iterable of all the directions. Doesn't create additional allocations.
        /// </summary>
        /// <returns>An IEnumerable with each direction, in order of increasing x, y</returns>
        public static IEnumerable<Direction> All() => Offsets.Keys;
    }
}
