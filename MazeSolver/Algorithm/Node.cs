using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MazeSolver.Algorithm
{
    internal class Node : FastPriorityQueueNode
    {
        /// <summary>
        /// Node ID. Needed to implement the IComparable interface correctly.
        /// This is calculated using the image dimensions and position in the constructor.
        /// </summary>
        public readonly int ID;

        /// <summary>
        /// Parent node X coordinate. A -1 means no parent node information.
        /// </summary>
        public int ParentX { get; private set; }

        /// <summary>
        /// Parent node Y coordinate. A -1 means no parent node information.
        /// </summary>
        public int ParentY { get; private set; }

        /// <summary>
        /// Node X coordinate.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Node Y Coordinate.
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// Whether or not this node can be placed on the open list.
        /// </summary>
        public bool Walkable { get; private set; } = false;

        /// <summary>
        /// Flag indicating that this node has been placed on the open list.
        /// This was added because checking a flag is faster than searching the open list.
        /// </summary>
        public bool Open { get; private set; } = false;

        /// <summary>
        /// Flag indicating that this node has been placed on the closed list.
        /// This was implemented in lieu of an actual list.
        /// </summary>
        public bool Closed { get; private set; } = false;

        /// <summary>
        /// Pixel color.
        /// </summary>
        public readonly Color Color;

        /// <summary>
        /// Total path score.
        /// </summary>
        public int F => G + H;

        /// <summary>
        /// Basic movement cost.
        /// </summary>
        public int G { get; private set; }

        /// <summary>
        /// Heuristic cost. Represents a weight for the distance to the goal.
        /// </summary>
        public int H { get; private set; }

        /// <summary>
        /// Constructor method for the class. Initializes all the needed fields of the class.
        /// </summary>
        /// <param name="x">X-axis pixel position of the node in the original image.</param>
        /// <param name="y">Y-axis pixel position of the node in the original image.</param>
        /// <param name="imageWidth">Width of the image. Needed for calculating a unique node ID.</param>
        /// <param name="color">Pixel color.</param>
        public Node(int x, int y, int imageWidth, Color color)
        {
            X = x;
            Y = y;
            ParentX = -1;
            ParentY = -1;
            ID = imageWidth * y + x;

            Color = color;
            if (color.IsConsideredRed() || color.IsConsideredBlue() || color.IsConsideredWhite())
            {
                Walkable = true;
            }
        }

        public bool HasParent => ParentX != -1 && ParentY != -1;

        public bool IsAt(Point point) => X == point.X && Y == point.Y;

        /// <summary>
        /// Class method for calculating the initial heuristic, movement cost, and total function cost.
        /// </summary>
        /// <param name="node">The parent node that is examining this node.</param>
        /// <param name="targetCoordinates">The goal coordinates for the pathfinding algorithm.</param>
        internal void CalculateF(Node node, Point targetCoordinates)
        {
            ParentX = node.X;
            ParentY = node.Y;

            // I'm using 14 and 10 instead of 1.0 and 1.414 to avoid double operations and square roots
            // in an effort to improve performance.
            if (node.X != X && node.Y != Y)
            {
                G = node.G + 14;
            }
            else
            {
                G = node.G + 10;
            }

            int distanceX = Math.Abs(X - targetCoordinates.X);
            int distanceY = Math.Abs(Y - targetCoordinates.Y);

            //Using diagonal shortcut heuristics.
            //The shorter axis limits how many nodes you can theoretically move diagonally.
            //The remainder must be traveled horizontally or vertically.
            if (distanceY < distanceX)
            {
                H = 14 * distanceY + 10 * (distanceX - distanceY);
            }
            else
            {
                H = 14 * distanceX + 10 * (distanceY - distanceX);
            }
        }

        /// <summary>
        /// If a node is already on the open list, it needs to have its G recalculated to see if a lower F can be found for the node from this alternate route.
        /// Returns whether or not the G was lower.
        /// </summary>
        /// <param name="node">The parent node that is examining this node.</param>
        /// <param name="targetCoordinates">The goal coordinates for the pathfinding algorithm.</param>
        /// <returns>True if the node was updated to reflect a better parent node, otherwise false.</returns>
        internal bool ReCalculateG(Node node)
        {
            int newG;

            if (node.X != X && node.Y != Y)
            {
                newG = node.G + 14;
            }
            else
            {
                newG = node.G + 10;
            }

            if (newG >= G)
                return false;

            ParentX = node.X;
            ParentY = node.Y;

            G = newG;

            return true;
        }

        internal void MarkAsStartPoint()
        {
            G = 0;

            // -1 will indicate it's the start point later on.
            ParentX = -1;
            ParentY = -1;
        }

        internal void MarkClosed()
        {
            Closed = true;
            Open = false;
        }

        internal void MarkOpen()
        {
            Open = true;
            Closed = false;
        }

        public override string ToString() => $"{{ {X},{Y} }}";
    }
}
