using TorchSharp;
using YueYinqiu.Su.TorchSharpUtilities;
using YueYinqiu.Su.TorchSharpUtilities.Checkpoints;
using YueYinqiu.Su.TorchSharpUtilities.Extensions;
using static TorchSharp.torch;

namespace Tests;
internal sealed class TensorCheckpoint(Tensor tensor) : IWrapped<Tensor>, ICheckpoint<TensorCheckpoint>
{
    public Tensor Value { get; } = tensor;
    public static TensorCheckpoint Load(FileInfo file)
    {
        return new(Tensor.Load(file.FullName));
    }
    public void Save(FileInfo file)
    {
        this.Value.SaveWithDirectory(file);
    }
    public override string ToString()
    {
        return this.Value.cstr();
    }
}
