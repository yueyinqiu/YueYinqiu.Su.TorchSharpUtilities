using TorchSharp.Modules;
using static TorchSharp.torch.nn;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class ModuleListCreateExtensions
{
    public static ModuleList<T> ToModuleList<T>(this IEnumerable<T> modules) where T : Module
    {
        return ModuleList(modules.ToArray());
    }
}
