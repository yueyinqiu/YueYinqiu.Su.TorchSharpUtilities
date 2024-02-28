using TorchSharp;
using static TorchSharp.torch;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class TensorPrintExtensions
{
    public static void Print(
        this Tensor tensor,
        TensorStringStyle style = TorchSharp.TensorStringStyle.CSharp)
    {
        Console.WriteLine(tensor.ToString(style));
    }
}
