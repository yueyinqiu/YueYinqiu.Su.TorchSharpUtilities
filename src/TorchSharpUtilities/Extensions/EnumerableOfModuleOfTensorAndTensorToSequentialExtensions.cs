using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorchSharp;
using TorchSharp.Modules;
using static TorchSharp.torch.nn;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class EnumerableOfModuleOfTensorAndTensorToSequentialExtensions
{
    public static Sequential ToSequential(
        this IEnumerable<Module<torch.Tensor, torch.Tensor>> modules)
    {
        return Sequential(modules);
    }
}
