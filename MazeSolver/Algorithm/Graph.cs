using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace MazeSolver.Algorithm
{
    internal class Graph
    {
        enum RGBColorMask { Blue = 0, Green = 1, Red = 2 };

        readonly Node[,] _graph;
        private Graph(Node[,] graph, DesignatedArea startingArea, DesignatedArea endingArea)
        {
            this._graph = graph;
            StartPoint = startingArea.GetCorrectedAveragePoint();
            EndPoint = endingArea.GetCorrectedAveragePoint();
        }

        public Node GetNode(Point point) => GetNode(point.X, point.Y);
        public Node GetNode(int x, int y) => _graph[x, y];
        public Node GetParent(Node node) => GetNode(node.ParentX, node.ParentY);

        public readonly Point StartPoint;
        public readonly Point EndPoint;

        public int Width => _graph.GetLength(0);
        public int Height => _graph.GetLength(1);

        public IEnumerable<Node> GetNeighbors(Node node)
        {
            var neighbors = new List<Node>();
            return Enum.GetValues(typeof(Direction)).Cast<Direction>()
                .Where(direction =>
                {
                    var x = direction.GetOffset().X + node.X;
                    var y = direction.GetOffset().Y + node.Y;
                    return x >= 0 && x < Width && y >= 0 && y < Height;
                }
                ).Select(direction => GetNeighbor(node, direction));
        }

        public Node GetNeighbor(Node node, Direction direction) => _graph[direction.GetOffset().X + node.X, direction.GetOffset().Y + node.Y];

        public static Graph Build(Bitmap sourceImage)
        {
            var startingArea = new DesignatedArea();
            var endingArea = new DesignatedArea();
            var graph = ProcessImage(sourceImage, startingArea, endingArea);

            if (!startingArea.HasPoints) throw new FormatException("The bitmap doesn't appear to have any valid starting point marked.");
            if (!endingArea.HasPoints) throw new FormatException("The bitmap doesn't appear to have any valid ending point marked.");

            return new Graph(graph, startingArea, endingArea);
        }

        /// <summary>
        /// Method for constructing the graph to be traversed by the program.
        /// Populates the graph nodes, the starting area, and the ending area.
        /// </summary>
        /// <param name="sourceImage">The original maze image file, in 24bpp bitmap format.</param>
        /// <param name="startingArea">Area to be populated with start points</param>
        /// <param name="endingArea">Area to be populated with end points</param>
        /// <returns></returns>
        private static Node[,] ProcessImage(Bitmap sourceImage, DesignatedArea startingArea, DesignatedArea endingArea)
        {
            int imageWidth = sourceImage.Width;
            int imageHeight = sourceImage.Height;

            Node[,] mazeGraph;
            mazeGraph = new Node[imageWidth, imageHeight];

            // Counters for processing the image.
            int currentByteCounter = 0;
            int pixelX = 0;
            int pixelY = 0;

            // Default RGB values.
            int blue = 0;
            int green = 0;
            int red = 0;
            var currentColor = new Color();

            // Using LockBits for lower level data access
            var sourceImageData = sourceImage.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadWrite, sourceImage.PixelFormat);

            // This is an unmanaged pointer and, with the way Marshal Copy works, it's clearer to copy the data into a 1-dimensional array and iterate over that instead.
            // This could be done slightly more efficiently with unsafe code and pointer manipulation, which would avoid this initial copy,
            // but the memory and performance impact shouldn't be super high -- the actual A* algorithm is by far the slowest part of the application.
            nint linePointer = sourceImageData.Scan0; // Pointer to the first pixel data in the bitmap.
            int strideLength = Math.Abs(sourceImageData.Stride);
            int numberOfBytes = strideLength * imageHeight;
            byte[] bitmapRGBValues = new byte[numberOfBytes];
            Marshal.Copy(linePointer, bitmapRGBValues, 0, numberOfBytes);

            // Processed as 3 bytes per pixel.
            foreach (byte currentByte in bitmapRGBValues)
            {
                int currentRowPosition = currentByteCounter - pixelY * strideLength;

                // The stride can be greater than the actual pixel data in each row.
                // If the current row position is past where there should be pixel data, do nothing.
                if (currentRowPosition < imageWidth * 3)
                {
                    switch (currentRowPosition % 3)
                    {
                        case (int)RGBColorMask.Blue:
                            blue = Convert.ToInt32(currentByte);
                            break;
                        case (int)RGBColorMask.Green:
                            green = Convert.ToInt32(currentByte);
                            break;
                        case (int)RGBColorMask.Red:
                            red = Convert.ToInt32(currentByte);

                            currentColor = Color.FromArgb(red, green, blue);
                            mazeGraph[pixelX, pixelY] = new Node(pixelX, pixelY, imageWidth, currentColor);

                            if (currentColor.IsConsideredRed())
                            {
                                startingArea.AddPoint(pixelX, pixelY);
                            }
                            else if (currentColor.IsConsideredBlue())
                            {
                                endingArea.AddPoint(pixelX, pixelY);
                            }

                            pixelX++;
                            break;
                    }
                }
                else
                {
                    //When we get to the last byte of the stride, we need to adjust counters for the next row
                    if (currentRowPosition + 1 == strideLength && pixelX == imageWidth)
                    {
                        pixelX = 0;
                        pixelY++;
                    }
                    // Edge case where stride length == image width * 3
                    // In this case, when the current row position is equal to the stride length, we must process this as the first byte of the next pixel.
                    else if (currentRowPosition == strideLength && pixelX == imageWidth)
                    {
                        pixelX = 0;
                        pixelY++;
                        blue = Convert.ToInt32(currentByte);
                    }
                }

                currentByteCounter++;
            }

            sourceImage.UnlockBits(sourceImageData);

            return mazeGraph;
        }

        /// <summary>
        /// Method for transcribing the path found to the bitmap image.
        /// </summary>
        /// <param name="mazeImage">Original source image. Needs to be a 24ppi bmp.</param>
        /// <returns>The generated bitmap with the path drawn in green.</returns>
        public Bitmap DrawToBitmap(Bitmap mazeImage)
        {
            int imageWidth = mazeImage.Width;
            int imageHeight = mazeImage.Height;
            var currentNode = GetNode(EndPoint);

            BitmapData imageData = mazeImage.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadWrite, mazeImage.PixelFormat);

            // See comments in ProcessImage
            var linePointer = imageData.Scan0;

            int strideLength = Math.Abs(imageData.Stride);
            int numberOfBytes = strideLength * imageHeight;

            var bitmapRGBValues = new byte[numberOfBytes];
            Marshal.Copy(linePointer, bitmapRGBValues, 0, numberOfBytes);

            Color currentColor = new();
            while (currentNode.HasParent)
            {
                //Console.WriteLine($"Drawing Node: {currentNode}");

                // 3 bytes per pixel.
                int pixelBytePosition = strideLength * currentNode.Y + 3 * currentNode.X;

                currentColor = Color.FromArgb(
                        bitmapRGBValues[pixelBytePosition + 2],
                        bitmapRGBValues[pixelBytePosition + 1],
                        bitmapRGBValues[pixelBytePosition]
                    );

                //No reason to color over the start and stop colors.
                if (!(currentColor.IsConsideredRed() || currentColor.IsConsideredBlue()))
                {
                    bitmapRGBValues[pixelBytePosition] = 0;
                    bitmapRGBValues[pixelBytePosition + 1] = 255;
                    bitmapRGBValues[pixelBytePosition + 2] = 0;
                }

                currentNode = GetParent(currentNode);
            }

            //Copy the modified RGB byte array back into memory and unlock the image.
            Marshal.Copy(bitmapRGBValues, 0, linePointer, numberOfBytes);
            mazeImage.UnlockBits(imageData);

            return mazeImage;
        }
    }
}
