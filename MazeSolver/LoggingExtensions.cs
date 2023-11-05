using Microsoft.Extensions.Logging;

namespace MazeSolver
{
    /// <summary>
    /// Logging configurations for the application
    /// </summary>
    internal class LoggingExtensions
    {
        public static readonly ILoggerFactory ConsoleFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
    }
}
