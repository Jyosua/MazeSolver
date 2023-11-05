using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver.Algorithm
{
    /// <summary>
    /// These extensions are what define our tolerances on what counts as paths, walls, and start/stop areas.
    /// </summary>
    internal static class ColorExtensions
    {
        public static bool IsConsideredRed(this Color color) => 255 - color.R < 56 && 255 - color.G > 128 && 255 - color.B > 128;
        public static bool IsConsideredBlue(this Color color) => 255 - color.R > 128 && 255 - color.G > 128 && 255 - color.B < 56;
        public static bool IsConsideredWhite(this Color color) => 255 - color.R < 20 && 255 - color.G < 20 && 255 - color.B < 20;
    }
}
