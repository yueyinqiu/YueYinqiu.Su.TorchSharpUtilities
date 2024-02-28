using TorchSharp;
using static TorchSharp.torch;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class TensorSaveExtensions
{
    public static void SaveWithDirectory(this Tensor tensor, string location, bool dotnetVersion = true)
    {
        var file = new FileInfo(location);
        file.Directory?.Create();
        if (dotnetVersion)
            tensor.Save(location);
        else
            tensor.save(location);
    }
}
