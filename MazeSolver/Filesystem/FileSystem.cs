namespace MazeSolver.Filesystem
{
    internal class FileSystem : IFileSystem
    {
        public FileSystem() { }

        public IFileData GetFileData(string path) => new FileData(path);
    }
}
