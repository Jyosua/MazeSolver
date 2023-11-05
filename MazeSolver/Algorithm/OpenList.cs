using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver.Algorithm
{
    internal class OpenList
    {
        // Originally I implemented this using a SortedSet which, when given duplicate priorities,
        // caused a preference for pixels towards the upper left of the image.
        // C# has an official PriorityQueue implementation as of C# 10, but it doesn't allow updating priorities,
        // nor removing an item that isn't next on the queue. So instead I'm using this library, which does.
        readonly FastPriorityQueue<Node> _openList;

        public OpenList(Graph graph)
        {
            _openList = new FastPriorityQueue<Node>(graph.Width * graph.Height);

            var startingNode = graph.GetNode(graph.StartPoint);
            startingNode.MarkAsStartPoint();
            _openList.Enqueue(startingNode, startingNode.F);
        }

        public Node Explore()
        {
            var node = _openList.Dequeue();
            node.MarkClosed();
            return node;
        }

        public void Add(Node node)
        {
            _openList.Enqueue(node, node.F);
            node.MarkOpen();
        }

        public void Update(Node node) => _openList.UpdatePriority(node, node.F);

        public bool IsEmpty() => _openList.Count == 0;
    }
}
