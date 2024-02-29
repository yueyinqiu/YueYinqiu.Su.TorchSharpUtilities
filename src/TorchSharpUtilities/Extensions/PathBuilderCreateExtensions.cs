namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class FileAndDirectoryCreatePathBuilderExtensions
{
    public static PathBuilder CreatePathBuilder(this FileInfo file)
    {
        return new PathBuilder(file.FullName);
    }
    public static PathBuilder CreatePathBuilder(this DirectoryInfo directory)
    {
        return new PathBuilder(directory.FullName);
    }
}
