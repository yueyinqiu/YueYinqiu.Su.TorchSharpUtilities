namespace Tests.Extensions;
public static class FileInfoExtensions
{
    public static void DeleteDirectory(this FileInfo fileInfo)
    {
        _ = fileInfo.Directory?.Deleted();
    }
}
