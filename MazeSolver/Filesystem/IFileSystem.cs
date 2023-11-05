namespace MazeSolver.Filesystem
{
    /// <summary>
    /// This interface is intended to allow for mocking of the filesystem
    /// </summary>
    public interface IFileSystem
    {
        IFileData GetFileData(string path);
    }
}