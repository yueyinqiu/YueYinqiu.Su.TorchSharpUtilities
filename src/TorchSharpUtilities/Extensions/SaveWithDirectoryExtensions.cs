using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class SaveWithDirectoryExtensions
{
    public static FileInfo SaveWithDirectory(
        this Module module, string location)
    {
        var file = new FileInfo(location);
        file.Directory?.Create();
        _ = module.save(location);
        return file;
    }

    public static FileInfo SaveWithDirectory(
        this Tensor tensor, string location, bool dotnetVersion = true)
    {
        var file = new FileInfo(location);
        file.Directory?.Create();
        if (dotnetVersion)
            tensor.Save(location);
        else
            tensor.save(location);
        return file;
    }
}
