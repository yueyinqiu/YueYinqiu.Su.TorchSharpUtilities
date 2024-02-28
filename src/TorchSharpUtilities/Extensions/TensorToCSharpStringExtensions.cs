using static TorchSharp.torch;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class TensorToCSharpStringExtensions
{
    public static string ToCsharpString(this Tensor tensor)
    {
        return tensor.ToString(TorchSharp.TensorStringStyle.CSharp);
    }
}
