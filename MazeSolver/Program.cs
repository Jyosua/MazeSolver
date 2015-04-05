using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

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

    class Program
    {
        static void Main(string[] args)
        {
            if (ArgCheck(args))
            {
                //Variable declarations.
                Node[,] mazeGraph;

                //I'm going to use SortedSets here, since they're implemented using self-balancing red-black trees.
                //Technically, the program could probably run slightly faster if I implemented my own priority queue,
                //but sorted sets exist in C# and are easier for a third party to read and understand.
                SortedSet<Node> openList = new SortedSet<Node>();
                SortedSet<Node> closedList = new SortedSet<Node>();

                //Read in the picture file.
                Bitmap sourceImage = new Bitmap(args[0]);

                //Convert the image to a matrix of node objects.

                //Code the A* pathfinding algorithm.

                //Either convert the matrix w/ path to an image, or draw on top of the old one.
            }
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

                        if (string.Compare(extension, "png", true) == 0 || string.Compare(extension, "bmp", true) == 0 || string.Compare(extension, "jpg", true) == 0)
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
