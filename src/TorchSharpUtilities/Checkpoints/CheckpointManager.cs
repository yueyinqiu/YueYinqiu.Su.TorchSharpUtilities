using YueYinqiu.Su.TorchSharpUtilities.Extensions;

namespace YueYinqiu.Su.TorchSharpUtilities.Checkpoints;
public sealed class CheckpointManager<T> where T : ICheckpoint<T>
{
    private readonly PathBuilder directory;
    public DirectoryInfo Directory => directory.AsDirectory();
    public string Prefix { get; }
    public string Suffix { get; }

    public CheckpointManager(
        DirectoryInfo directory,
        string prefix = "checkpoint",
        string suffix = ".cp")
    {
        directory.Create();
        this.directory = directory.CreatePathBuilder();
        Prefix = prefix;
        Suffix = suffix;

        // check prefix and suffix
        _ = GetFile(0);
    }

    private FileInfo GetFile(int index)
    {
        var builder = directory.Join($"{Prefix}{index}{Suffix}");
        return builder.AsFile();
    }

    private class WrappedCheckpoint(FileInfo file) : IWrapped<T>
    {
        public T Value => T.Load(file);
    }

    public IEnumerable<(int index, IWrapped<T> checkpoint)> ListAll()
    {
        foreach (var file in Directory.EnumerateFiles($"{Prefix}*{Suffix}"))
        {
            var indexString = file.Name[Prefix.Length..^Suffix.Length];
            if (int.TryParse(indexString, out var index))
                yield return (index, new WrappedCheckpoint(file));
        }
    }

    public T Load(int? index)
    {
        if (index.HasValue)
        {
            var file = GetFile(index.Value);
            return T.Load(file);
        }
        return ListAll().MaxBy(x => x.index).checkpoint.Value;
    }

    public void Save(int index, T checkpoint)
    {
        var file = GetFile(index);
        checkpoint.Save(file);
    }
}
