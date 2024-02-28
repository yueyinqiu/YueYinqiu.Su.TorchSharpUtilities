namespace YueYinqiu.Su.TorchSharpUtilities;

public sealed record PathBuilder(string Path)
{
    public static PathBuilder operator /(PathBuilder left, string right)
    {
        return new(System.IO.Path.Join(left.Path, right));
    }
    public static PathBuilder operator /(string left, PathBuilder right)
    {
        return new(System.IO.Path.Join(left, right.Path));
    }
    public static implicit operator string(PathBuilder path)
    {
        return path.Path;
    }

    public PathBuilder Join(string path)
    {
        return this / path;
    }

    public PathBuilder Full(string? basePath = null)
    {
        if (basePath is null)
            return new(System.IO.Path.GetFullPath(this.Path));
        else
            return new(System.IO.Path.GetFullPath(this.Path, basePath));
    }

    public DirectoryInfo AsDirectory()
    {
        return new DirectoryInfo(this.Path);
    }

    public FileInfo AsFile()
    {
        return new FileInfo(this.Path);
    }

    public override string ToString()
    {
        return this.Path;
    }

    public static PathBuilder GetSpecialFolder(Environment.SpecialFolder folder)
    {
        return new PathBuilder(Environment.GetFolderPath(folder)).Full();
    }

    public static PathBuilder MyDocuments => GetSpecialFolder(Environment.SpecialFolder.MyDocuments);
}
