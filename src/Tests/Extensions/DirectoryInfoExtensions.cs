namespace Tests.Extensions;
public static class DirectoryInfoExtensions
{
    public static DirectoryInfo Deleted(this DirectoryInfo directoryInfo)
    {
        if (directoryInfo.Exists)
            directoryInfo.Delete(true);
        return directoryInfo;
    }
}
