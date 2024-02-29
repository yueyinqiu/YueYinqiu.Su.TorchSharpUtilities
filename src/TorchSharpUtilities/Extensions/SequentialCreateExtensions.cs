using TorchSharp;
using TorchSharp.Modules;
using static TorchSharp.torch.nn;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class SequentialCreateExtensions
{
    public static Sequential ToSequential(
        this IEnumerable<Module<torch.Tensor, torch.Tensor>> modules)
    {
        return Sequential(modules);
    }
}
