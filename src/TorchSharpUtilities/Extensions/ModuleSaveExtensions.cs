using static TorchSharp.torch.nn;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class ModuleSaveExtensions
{
    public static void SaveWithDirectory(this Module module, string location)
    {
        var file = new FileInfo(location);
        file.Directory?.Create();
        _ = module.save(location);
    }
}
