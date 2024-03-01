using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class TensorAndModuleSaveExtensions
{
    public static void SaveWithDirectory(
        this Tensor tensor, FileInfo location, bool dotnetVersion = true)
    {
        location.Directory?.Create();
        if (dotnetVersion)
            tensor.Save(location.FullName);
        else
            tensor.save(location.FullName);
    }

    public static FileInfo SaveWithDirectory(
        this Tensor tensor, string location, bool dotnetVersion = true)
    {
        var file = new FileInfo(location);
        tensor.SaveWithDirectory(file, dotnetVersion);
        return file;
    }

    public static void SaveWithDirectory(
        this Module module, FileInfo location, IList<string>? skip = null)
    {
        location.Directory?.Create();
        _ = module.save(location.FullName, skip);
    }

    public static FileInfo SaveWithDirectory(
        this Module module, string location, IList<string>? skip = null)
    {
        var file = new FileInfo(location);
        module.SaveWithDirectory(file, skip);
        return file;
    }
}
