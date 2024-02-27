using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorchSharp;
using TorchSharp.Modules;
using static TorchSharp.torch.nn;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class EnumerableOfModuleToModuleListExtensions
{
    public static ModuleList<T> ToModuleList<T>(this IEnumerable<T> modules) where T : Module
    {
        return ModuleList(modules.ToArray());
    }
}
