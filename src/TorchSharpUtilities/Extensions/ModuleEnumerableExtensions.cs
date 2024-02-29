using TorchSharp;
using TorchSharp.Modules;
using static TorchSharp.torch.nn;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class ModuleEnumerableExtensions
{
    public static ModuleList<T> ToModuleList<T>(this IEnumerable<T> modules) where T : Module
    {
        return ModuleList(modules.ToArray());
    }
    public static Sequential ToSequential(
        this IEnumerable<Module<torch.Tensor, torch.Tensor>> modules)
    {
        return Sequential(modules);
    }
}
