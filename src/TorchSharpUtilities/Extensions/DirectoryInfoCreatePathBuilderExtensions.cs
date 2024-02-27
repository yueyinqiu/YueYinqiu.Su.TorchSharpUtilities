using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class DirectoryInfoCreatePathBuilderExtensions
{
    public static PathBuilder CreatePathBuilder(this DirectoryInfo directory)
    {
        return new PathBuilder(directory.FullName);
    }
}
