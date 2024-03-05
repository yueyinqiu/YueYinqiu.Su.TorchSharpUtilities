using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Extensions;
public static class FileInfoExtensions
{
    public static void DeleteDirectory(this FileInfo fileInfo)
    {
        _ = fileInfo.Directory?.Deleted();
    }
}
