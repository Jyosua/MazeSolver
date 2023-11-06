using MazeSolver.Algorithm;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MazeSolver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var logger = LoggingExtensions.ConsoleFactory.CreateLogger<Program>();

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.LogError("Due to reliance on System.Drawing.Bitmap, for now only Windows is supported");
                Environment.Exit(1);
            }

            var parser = new ArgumentParser(args);
            var argsValid = parser.Parse();
            logger.LogInformation($"Argument parsing result: {argsValid}");
            logger.LogInformation($"Input file: {parser.InputFilePath}");
            logger.LogInformation($"Output file: {parser.OutputFilePath}");

            if (!argsValid || parser.InputFilePath == null || parser.OutputFilePath == null)
            {
                Environment.Exit(1);
            }

            try
            {
                var sourceImage = PrepareBitmap(parser.InputFilePath);
                var graph = Graph.Build(sourceImage);
                var success = PathFinder.Find(graph);
            

                if(!success)
                {
                    logger.LogError("A connecting path could not be found!");
                    Environment.Exit(1);
                }

                graph.DrawToBitmap(sourceImage).Save(parser.OutputFilePath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unhandled exception was thrown.");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// This converts whatever image format the file was in to a 24bpp bitmap without an alpha channel.
        /// We don't care about alpha channel for our use case anyhow and this format is easy to manipulate.
        /// </summary>
        /// <param name="sourceFilePath">The file path of the source image.</param>
        /// <returns>A 24bpp rgb bitmap of the image.</returns>
        static Bitmap PrepareBitmap(string sourceFilePath)
        {
            using var originalSourceImage = new Bitmap(sourceFilePath);
            var sourceImage = new Bitmap(originalSourceImage.Width, originalSourceImage.Height, PixelFormat.Format24bppRgb);

            using Graphics drawingSurface = Graphics.FromImage(sourceImage);
            drawingSurface.DrawImage(originalSourceImage, new Rectangle(0, 0, sourceImage.Width, sourceImage.Height));

            return sourceImage;
        }
    }
}