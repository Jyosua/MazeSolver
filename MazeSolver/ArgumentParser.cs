using MazeSolver.Filesystem;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace MazeSolver
{
    public class ArgumentParser
    {
        private readonly ILogger<ArgumentParser> _logger;
        readonly IFileSystem _fileSystem;
        readonly string[] _originalArgs;

        const string _argumentFormatExample = """"MazeSolver.exe '[source filename].[bmp, png, jpg]' '[destination filename].[bmp, png, jpg]'"""";
        readonly ReadOnlyCollection<string> _allowedExtensions = new List<string>() { ".png", ".bmp", ".jpg" }.AsReadOnly();

        public ArgumentParser(string[] args) : this(args, LoggingExtensions.ConsoleFactory.CreateLogger<ArgumentParser>(), new FileSystem()) { }

        public ArgumentParser(string[] args, ILogger<ArgumentParser> logger, IFileSystem fileSystem)
        {
            _originalArgs = args;
            _logger = logger;
            _fileSystem = fileSystem;
        }

        public string? InputFilePath { get; set; }
        public string? OutputFilePath { get; set; }

        public bool Parse()
        {
            if (_originalArgs.Length != 2)
            {
                _logger.LogError($"Invalid number of arguments! Proper usage is: {_argumentFormatExample}");
                return false;
            }

            string inputPath = _originalArgs[0];
            string outputPath = _originalArgs[1];

            if(FilePathIsValid(inputPath, expectExists: true) && FilePathIsValid(outputPath, checkExtensions: false))
            {
                InputFilePath = inputPath;
                OutputFilePath = outputPath;
                return true;
            }

            return false;
        }

        private bool FilePathIsValid(string path, bool expectExists = false, bool checkExtensions = true)
        {
            IFileData fileInfo;
            try
            {
                fileInfo = _fileSystem.GetFileData(path);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid File Path: {ex}");
                return false;
            }

            if(!fileInfo.Exists && expectExists)
            {
                _logger.LogError($"Specified file does not exist! File: {path}");
                return false;
            }

            if (fileInfo.Exists && !expectExists)
            {
                // We could offer the ability to overwrite the file, but maybe later.
                // For now, we'll exit gracefully if the destination has an existing file.
                _logger.LogError($"File exists at destination! File: {path}\n\rPlease try again with an empty location.");
                return false;
            }

            if (!checkExtensions)
                return true;

            // This is a simplistic to check the extension.
            // Technically this will mean that improperly named files won't work,
            // but it's not really a big deal.
            string extension = fileInfo.Extension;
            if (string.IsNullOrEmpty(extension))
            {
                _logger.LogError($"The specified file has no extension! File: {path}");
                return false;
            }

            if (!_allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                _logger.LogError($"The specified file has an unsupported extension! File: {path}");
                return false;
            }

            return true;
        }
    }
}
