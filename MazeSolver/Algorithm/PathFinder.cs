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
    /// <summary>
    /// A* Pathfinding implementation
    /// </summary>
    internal class PathFinder
    {
        /// <summary>
        /// Pathfinding method. Implemented as A*.
        /// </summary>
        /// <param name="graph">The graph generated from the original image file.</param>
        /// <returns>True if a path was successfully found. If all nodes are exhausted before reaching the target, returns false.</returns>
        public static bool Find(Graph graph)
        {
            var pathFound = false;
            var openList = new OpenList(graph);

            // Tail recursive, so implementing as a loop
            while (!pathFound)
            {
                if (openList.IsEmpty())
                    break; // No Path Exists;

                var node = openList.Explore();
                if (node.IsAt(graph.EndPoint))
                {
                    pathFound = true;
                    break;
                }

                //Console.WriteLine("Current Node: " + node.X.ToString() + ", " + node.Y.ToString() + "\n");

                foreach (var direction in Directions.All())
                {
                    var offset = direction.GetOffset();
                    var x = offset.X + node.X;
                    var y = offset.Y + node.Y;
                    if (!(x >= 0 && x < graph.Width && y >= 0 && y < graph.Height))
                        continue;

                    //This is to prevent cutting corners and squeezing through walls
                    bool leftmostAdjacentWalkable = true;
                    bool rightmostAdjacentWalkable = true;

                    switch (direction)
                    {
                        case Direction.Northwest:
                            leftmostAdjacentWalkable = graph.GetNode(x, y + 1).Walkable;
                            rightmostAdjacentWalkable = graph.GetNode(x + 1, y).Walkable;
                            break;
                        case Direction.Northeast:
                            leftmostAdjacentWalkable = graph.GetNode(x - 1, y).Walkable;
                            rightmostAdjacentWalkable = graph.GetNode(x, y + 1).Walkable;
                            break;
                        case Direction.Southwest:
                            leftmostAdjacentWalkable = graph.GetNode(x, y - 1).Walkable;
                            rightmostAdjacentWalkable = graph.GetNode(x + 1, y).Walkable;
                            break;
                        case Direction.Southeast:
                            leftmostAdjacentWalkable = graph.GetNode(x - 1, y).Walkable;
                            rightmostAdjacentWalkable = graph.GetNode(x, y - 1).Walkable;
                            break;
                    }

                    if (!leftmostAdjacentWalkable || !rightmostAdjacentWalkable)
                        continue;

                    var neighbor = graph.GetNode(x, y);

                    if (!(neighbor.Walkable && !neighbor.Closed))
                        continue;

                    if (!neighbor.Open)
                    {
                        neighbor.CalculateF(node, graph.EndPoint);
                        openList.Add(neighbor);
                    }
                    else
                    {
                        neighbor.ReCalculateG(node);
                        openList.Update(neighbor);
                    }
                }
            }

            return pathFound;
        }
    }
}
