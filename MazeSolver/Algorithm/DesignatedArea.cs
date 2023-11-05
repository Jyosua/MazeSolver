using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver.Algorithm
{
    /// <summary>
    /// This is a class designed to represent the starting or ending areas of a given Maze
    /// </summary>
    internal class DesignatedArea
    {
        readonly HashSet<Point> _associatedPoints = new();
        int _xSums, _ySums = 0;
        public DesignatedArea() { }

        public void AddPoint(int x, int y)
        {
            _associatedPoints.Add(new Point(x, y));
            _xSums += x;
            _ySums += y;
        }

        public bool HasPoints => _associatedPoints.Count > 0;

        /// <summary>
        /// This method tries to return the most suitable average point for the area.
        /// </summary>
        /// <returns>The average or nearest neighbor point if the actual average isn't valid.
        /// Otherwise, it returns the first point in the area.</returns>
        public Point GetCorrectedAveragePoint()
        {
            if (_associatedPoints.Count == 0) throw new InvalidOperationException($"{nameof(GetCorrectedAveragePoint)} was called without adding any points!");

            if (_associatedPoints.Count == 1) return _associatedPoints.First();

            var averagePoint = GetAveragePoint(_associatedPoints, _xSums, _ySums);
            var valid = AdjustAveragePoint(_associatedPoints, ref averagePoint);

            return valid ? averagePoint : _associatedPoints.First();
        }

        public static Point GetAveragePoint(HashSet<Point> points, int xSums, int ySums)
        {
            if (points.Count == 0) throw new InvalidOperationException($"{nameof(GetAveragePoint)} was called without adding any points!");

            if (points.Count == 1) return points.First();

            double averageX = xSums / (double)points.Count;
            double averageY = ySums / (double)points.Count;
            return new Point(Convert.ToInt32(Math.Round(averageX)), Convert.ToInt32(Math.Round(averageY)));
        }

        // Since it's an average, we need to check that the point is actually in the list
        // If it's not, we'll search neighboring points for a valid point and use one of those
        // If nothing is found, return false
        public static bool AdjustAveragePoint(HashSet<Point> points, ref Point averagePoint)
        {
            if (points.Contains(averagePoint))
                return true;

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x != 0 && y != 0)
                    {
                        Point testPoint = new(averagePoint.X + x, averagePoint.Y + y);
                        if (points.Contains(testPoint))
                        {
                            averagePoint.X = testPoint.X;
                            averagePoint.Y = testPoint.Y;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
