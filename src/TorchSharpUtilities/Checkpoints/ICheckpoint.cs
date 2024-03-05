namespace YueYinqiu.Su.TorchSharpUtilities.Checkpoints;
public interface ICheckpoint<TSelf> where TSelf : ICheckpoint<TSelf>
{
    void Save(FileInfo file);
    static abstract TSelf Load(FileInfo file);
}
