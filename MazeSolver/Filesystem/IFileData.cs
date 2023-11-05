namespace MazeSolver.Filesystem
{
    /// <summary>
    /// This is to allow mocking of operations around FileInfo
    /// </summary>
    public interface IFileData
    {
        bool Exists { get; }
        string Extension { get; }
    }
}