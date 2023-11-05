namespace MazeSolver.Filesystem
{
    /// <summary>
    /// Wrapper class for FileInfo because it doesn't have an interface I can use as is
    /// </summary>
    internal class FileData : IFileData
    {
        readonly FileInfo _file;
        public FileData(string path)
        {
            _file = new FileInfo(path);
        }

        public bool Exists => _file.Exists;

        public string Extension => _file.Extension;
    }
}
