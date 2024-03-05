using YueYinqiu.Su.TorchSharpUtilities.Extensions;

namespace YueYinqiu.Su.TorchSharpUtilities.Checkpoints;
public sealed class CheckpointManager<T> where T : ICheckpoint<T>
{
    private readonly PathBuilder directory;
    public DirectoryInfo Directory => this.directory.AsDirectory();
    public string Prefix { get; }
    public string Suffix { get; }

    public CheckpointManager(
        DirectoryInfo directory,
        string prefix = "checkpoint",
        string suffix = ".cp")
    {
        directory.Create();
        this.directory = directory.CreatePathBuilder();
        this.Prefix = prefix;
        this.Suffix = suffix;

        // check prefix and suffix
        _ = this.GetFile(0);
    }

    private FileInfo GetFile(int index)
    {
        var builder = this.directory.Join($"{this.Prefix}{index}{this.Suffix}");
        return builder.AsFile();
    }

    private class WrappedCheckpoint(FileInfo file) : IWrapped<T>
    {
        public T Value => T.Load(file);
    }

    public IEnumerable<(int index, IWrapped<T> checkpoint)> ListAll()
    {
        foreach (var file in this.Directory.EnumerateFiles($"{this.Prefix}*{this.Suffix}"))
        {
            var indexString = file.Name[this.Prefix.Length..^this.Suffix.Length];
            if (int.TryParse(indexString, out var index))
                yield return (index, new WrappedCheckpoint(file));
        }
    }

    public T Load(int? index)
    {
        if (index.HasValue)
        {
            var file = this.GetFile(index.Value);
            return T.Load(file);
        }
        return this.ListAll().MaxBy(x => x.index).checkpoint.Value;
    }

    public void Save(int index, T checkpoint)
    {
        var file = this.GetFile(index);
        checkpoint.Save(file);
    }
}
