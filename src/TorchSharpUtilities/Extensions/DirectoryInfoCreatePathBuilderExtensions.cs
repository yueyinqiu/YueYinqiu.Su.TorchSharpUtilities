namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class DirectoryInfoCreatePathBuilderExtensions
{
    public static PathBuilder CreatePathBuilder(this DirectoryInfo directory)
    {
        return new PathBuilder(directory.FullName);
    }
}
