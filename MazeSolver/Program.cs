using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MazeSolver
{
    /// <summary>
    /// The Node class is all of the information related to a single pixel in the loaded image.
    /// We are implementing the IComparable interface because the SortedSet we're using as a priority queue needs to know how to order the Nodes properly.
    /// </summary>
    class Node : IComparable<Node>
    {
        /// <summary>
        /// Node ID. Needed to implement the IComparable interface correctly.
        /// This is calculated using the image dimensions and position in the constructor.
        /// </summary>
        public int ID
        {
            get;
            private set;
        }

        /// <summary>
        /// Parent node X coordinate.
        /// </summary>
        public int ParentX
        {
            get;
            private set;
        }

        /// <summary>
        /// Parent node Y coordinate.
        /// </summary>
        public int ParentY
        {
            get;
            private set;
        }

        /// <summary>
        /// Node X coordinate.
        /// </summary>
        public int X
        {
            get;
            private set;
        }

        /// <summary>
        /// Node Y Coordinate.
        /// </summary>
        public int Y
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether or not this node can be placed on the open list.
        /// </summary>
        public bool Walkable
        {
            get;
            private set;
        }

        /// <summary>
        /// Pixel color.
        /// </summary>
        public Color Color
        {
            get;
            private set;
        }

        /// <summary>
        /// Total path score.
        /// </summary>
        public int F
        {
            get;
            private set;
        }

        /// <summary>
        /// Movement cost.
        /// </summary>
        public int G
        {
            get;
            private set;
        }

        /// <summary>
        /// Heuristic cost.
        /// </summary>
        public int H
        {
            get;
            private set;
        }

        public Node(int x, int y, int imageWidth, Color pixelColor)
        {
            this.X = x;
            this.Y = y;
            this.ID = (imageWidth * y) + x;

            this.Color = pixelColor;

            if (pixelColor == Color.FromArgb(255, 0, 0) || pixelColor == Color.FromArgb(0, 0, 255) || pixelColor == Color.FromArgb(255, 255, 255))
            {
                Walkable = true;
            }
            else
            {
                Walkable = false;
            }
        }

        /// <summary>
        /// IComparable method implementation for use with the SortedSets.
        /// </summary>
        /// <param name="that">The Node you're comparing against.</param>
        /// <returns>Whether "this" Node should precede "that" node in the Sort order. -1 precedes, 1 follows, and 0 indicates the same Node.</returns>
        public int CompareTo(Node that)
        {
            if (this.F < that.F)
            {
                return -1;
            }
            else if (this.F > that.F)
            {
                return 1;
            }
            else
            {
                //SortedSet throws away duplicates, so we need to compare IDs to ensure that different Nodes with equal weights are not tossed.
                if (this.ID == that.ID)
                {
                    return 0;
                }
                else
                {
                    //We want to prioritize items added to the list last.
                    //If the Node isn't actually a duplicate, later items are of greater priority.
                    return -1;
                }
            }
        }
    }

    enum RGBColorMask { Blue = 0, Green = 1, Red = 2};

    class Program
    {
        static void Main(string[] args)
        {
            if (ArgCheck(args))
            {
                #region Variable Declarations
                Node[,] mazeGraph;

                //These are for calculating the average start and stop points.
                List<Point> startPoints = new List<Point>();
                List<Point> endPoints = new List<Point>();
                Point averageStartPoint = new Point();
                Point averageEndPoint = new Point();

                //I'm going to use SortedSets here, since they're implemented using self-balancing red-black trees.
                //Technically, the program could probably run slightly faster if I implemented my own priority queue,
                //but sorted sets exist in C# and are easier for a third party to read and understand.
                SortedSet<Node> openList = new SortedSet<Node>();
                SortedSet<Node> closedList = new SortedSet<Node>();
                #endregion

                //Build the graph from the source image and calculate average start/end points. Lists and Points are modified.
                mazeGraph = BuildNodeGraph(args, startPoints, endPoints, averageStartPoint, averageEndPoint);
                
                //Find starting and ending pixels.
                bool checkStartStop = false;
                if(averageStartPoint != null && averageEndPoint != null)
                {
                    checkStartStop = CheckStartStopCriteria(startPoints, endPoints, averageStartPoint, averageEndPoint);
                }
                else
                {
                    Console.WriteLine("Start/Stop criteria missing! Cannot begin pathfinding!");
                    return;
                }

                //Code the A* pathfinding algorithm.
                if (checkStartStop)
                {
                    //WE'RE IN BUSINESS!
                }

                //Either convert the matrix w/ path to an image, or draw on top of the old one.
            }

            return;
        }

        /// <summary>
        /// Method for verifying the start and stop points calculated by the graph generation.
        /// If they are incorrect, the method searches for a correct start/stop point.
        /// </summary>
        /// <param name="startPoints">List of valid start points.</param>
        /// <param name="endPoints">List of valid end points.</param>
        /// <param name="averageStartPoint">Postulated average start point.</param>
        /// <param name="averageEndPoint">Postulated average end point.</param>
        /// <returns></returns>
        private static bool CheckStartStopCriteria(List<Point> startPoints, List<Point> endPoints, Point averageStartPoint, Point averageEndPoint)
        {
            //First, verify that the calculated average start point is actually valid.
            //If it's not, we'll look at all adjacent pixels for one that is present in the list of valid start points.
            //Normally it would be inadviseable to search large lists, as they are O(n) in the worst case.
            //However, as it turns out, the larger the list, the greater the chance you only have to do this once.
            //Despite this, consider replacing with a dictionary, instead.
            bool startPointFound = false;

            if(!startPoints.Contains(averageStartPoint))
            {
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        if(x!=0 && y!=0)
                        {
                            int testX = averageStartPoint.X + x;
                            int testY = averageStartPoint.Y + y;
                            Point testPoint = new Point(testX, testY);
                            if (startPoints.Contains(testPoint))
                            {
                                averageStartPoint = testPoint;
                                startPointFound = true;
                                goto Start_Point_Search_Complete; //Dijkstra rolls over in his grave.
                            }
                        }
                    }
                }
            }
            else
            {
                startPointFound = true;
            }

            //A very rare use of a goto to break out of the double loops. Recommended in the MSDN reference library -- go figure!
            //If a start point still wasn't found, it may indicate larger problems...
            Start_Point_Search_Complete:
                if(!startPointFound)
                {
                    return false;
                }

            bool endPointFound = false;

            if (!endPoints.Contains(averageEndPoint))
            {
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        if (x != 0 && y != 0)
                        {
                            int testX = averageEndPoint.X + x;
                            int testY = averageEndPoint.Y + y;
                            Point testPoint = new Point(testX, testY);
                            if (startPoints.Contains(testPoint))
                            {
                                averageEndPoint = testPoint;
                                endPointFound = true;
                                goto End_Point_Search_Complete;
                            }
                        }
                    }
                }
            }
            else
            {
                endPointFound = true;
            }

            //Same as before, but this time for the end point.
            End_Point_Search_Complete:
                if (startPointFound && endPointFound)
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }

        /// <summary>
        /// Method for constructing the graph to be traversed by the program.
        /// Also modifies the startPoints and endPoints lists by populating them with potential start/end points.
        /// </summary>
        /// <param name="args">The original program arguments.</param>
        /// <returns>The generated Node array.</returns>
        private static Node[,] BuildNodeGraph(string[] args, List<Point> startPoints, List<Point> endPoints, Point averageStartPoint, Point averageEndPoint)
        {
            Node[,] localMazeGraph;
            int startingPointXSum = 0, startingPointYSum = 0, numberOfStartingPoints = 0;
            int endingPointXSum = 0, endingPointYSum = 0, numberOfEndingPoints = 0;

            //Read in the picture file and convert to a common format.
            //A 24bpp should be sufficient and is easy to work with. We also don't care about alpha channel data.
            Bitmap preconvertSourceImage = new Bitmap(args[0]);
            Bitmap sourceImage = new Bitmap(preconvertSourceImage.Width, preconvertSourceImage.Height, PixelFormat.Format24bppRgb);
            using (Graphics drawingSurface = Graphics.FromImage(sourceImage))
            {
                drawingSurface.DrawImage(preconvertSourceImage, new Rectangle(0, 0, sourceImage.Width, sourceImage.Height));
            }

            //Don't need the original bitmap anymore, so might as well free the memory.
            preconvertSourceImage.Dispose();

            int imageWidth = sourceImage.Width;
            int imageHeight = sourceImage.Height;
            localMazeGraph = new Node[imageWidth, imageHeight];

            //Creating a BitmapData object for reading and future manipulation of the image.
            Rectangle imageDimensions = new Rectangle(0, 0, imageWidth, imageHeight);
            BitmapData sourceImageData = sourceImage.LockBits(imageDimensions, System.Drawing.Imaging.ImageLockMode.ReadWrite, sourceImage.PixelFormat);

            //First pixel data in the bitmap.
            //This is an unmanaged pointer and, with the way Marshal Copy works, it's probably just easier to copy the data into a 1-dimensional array first.
            //This could be done more efficiently with unsafe code and pointer manipulation.
            IntPtr linePointer = sourceImageData.Scan0;
            int strideLength = Math.Abs(sourceImageData.Stride);
            int numberOfBytes = strideLength * imageHeight;
            byte[] bitmapRGBValues = new byte[numberOfBytes];

            //Copy the data into our byte array.
            Marshal.Copy(linePointer, bitmapRGBValues, 0, numberOfBytes);

            //Process as 3 bytes per pixel.
            int currentByteCounter = 0;
            int pixelX = 0;
            int pixelY = 0;

            //Default RGB values.
            int blue = 0;
            int green = 0;
            int red = 0;
            Color currentColor = new Color();

            foreach (byte currentByte in bitmapRGBValues)
            {
                int currentRowPosition = currentByteCounter - (pixelY * strideLength);

                //The stride can be greater than the actual pixel data in each row.
                //If the current row position is past where there should be pixel data, do nothing.
                if (currentRowPosition < (sourceImage.Width * 3))
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
                            //Create the node
                            localMazeGraph[pixelX, pixelY] = new Node(pixelX, pixelY, imageWidth, currentColor);

                            if (currentColor == Color.FromArgb(255, 0, 0))
                            {
                                //Add coordinates to potential start point list
                                startPoints.Add(new Point(pixelX, pixelY));
                                startingPointXSum += pixelX;
                                startingPointYSum += pixelY;
                                numberOfStartingPoints++;
                            }else if(currentColor == Color.FromArgb(0,0,255))
                            {
                                //Add coordinates to potential end point list
                                endPoints.Add(new Point(pixelX, pixelY));
                                endingPointXSum += pixelX;
                                endingPointYSum += pixelY;
                                numberOfEndingPoints++;
                            }

                            pixelX++;
                            break;
                    }
                }
                else
                {
                    //When we get to the last byte of a row, we need to reset the X coordinate counter and increment the Y.
                    if ((currentRowPosition + 1) == strideLength && pixelX == imageWidth)
                    {
                        pixelX = 0;
                        pixelY++;
                    }
                    //Edge case where stride = image width * 3
                    //In this case, when the current row position is equal to the stride length, we must process this as the first byte of the next pixel.
                    else if (currentRowPosition == strideLength && pixelX == imageWidth)
                    {
                        pixelX = 0;
                        pixelY++;
                        blue = Convert.ToInt32(currentByte);
                    }
                }

                currentByteCounter++;
            }

            if (numberOfStartingPoints!=0)
            {
                double averageStartX = (double)startingPointXSum/(double)numberOfStartingPoints;
                double averageStartY = (double)startingPointYSum / (double)numberOfStartingPoints;
                averageStartPoint = new Point(Convert.ToInt32(Math.Round(averageStartX)), Convert.ToInt32(Math.Round(averageStartY)));
            }
            else
            {
                Console.WriteLine("No start points found!");
            }

            if (numberOfEndingPoints != 0)
            {
                double averageEndX = (double)endingPointXSum / (double)numberOfEndingPoints;
                double averageEndY = (double)endingPointYSum / (double)numberOfEndingPoints;
                averageStartPoint = new Point(Convert.ToInt32(Math.Round(averageEndX)), Convert.ToInt32(Math.Round(averageEndY)));
            }
            else
            {
                Console.WriteLine("No end points found!");
            }

            return localMazeGraph;
        }

        /// <summary>
        /// Helper method to check the arguments provided via command line.
        /// </summary>
        /// <param name="args">The arguments the user provided at execution.</param>
        /// <returns>Returns true if the arguments were valid, otherwise false.</returns>
        static bool ArgCheck(string[] args)
        {
            bool validSource = false;
            bool validDestination = false;

            //We're only expecting two arguements.
            if (args.Length == 2)
            {
                string inputPath = args[0];
                string outputPath = args[1];


                //This block is testing for a valid source filename
                FileInfo inputFI = null;
                bool inputFilenameException = false;

                try
                {
                    inputFI = new FileInfo(inputPath);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Invalid Source Path: " + ex.ToString());
                    inputFilenameException = true;
                }
                
                //If the filename is valid, we need to know if it exists.
                if(!inputFilenameException)
                {
                    if (File.Exists(inputPath))
                    {
                        //The file exists, so we need to know if it's of the right type, now.
                        //Technically this will mean that improperly named files won't work,
                        //but it's not really a big deal.
                        string extension = Path.GetExtension(inputPath);

                        if (string.Compare(extension, ".png", true) == 0 || string.Compare(extension, ".bmp", true) == 0 || string.Compare(extension, ".jpg", true) == 0)
                        {
                            //Hurrah! We've got a valid source image!
                            validSource = true;
                        }
                        else
                        {
                            Console.WriteLine("Incorrect file type.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Source file does not exist!");
                    }
                }

                //Now we check the destination filename.
                //No real reason to do so if we don't have a valid source, anyhow.
                if (validSource)
                {
                    //This block is testing for a valid destination filename
                    FileInfo outputFI = null;
                    bool outputFilenameException = false;

                    try
                    {
                        outputFI = new FileInfo(outputPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Invalid Source Path: " + ex.ToString());
                        outputFilenameException = true;
                    }

                    //If the filename is valid, we need to know if it exists.
                    if (!outputFilenameException)
                    {
                        if (File.Exists(outputPath))
                        {
                            //The destination file already exists, which is NOT what we want.
                            //We could offer the ability to overwrite the file, but maybe later.
                            //For now, we'll exit gracefully if the destination has an existing file.
                            Console.WriteLine("File exists at destination!\n\rPlease try again with an empty location.");
                        }
                        else
                        {
                            //We could check the extension on the destination file, but we'll trust that the user knows what they want to name the file.
                            validDestination = true;
                        }
                    }
                }

                
            }
            else
            {
                Console.WriteLine("Invalid Arguments!");
            }

            if (validSource && validDestination)
            {
                return true;
            }
            else
                return false;
        }
    }
}
