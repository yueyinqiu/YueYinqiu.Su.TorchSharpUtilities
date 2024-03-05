using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Extensions;
public static class DirectoryInfoExtensions
{
    public static DirectoryInfo Deleted(this DirectoryInfo directoryInfo)
    {
        if (directoryInfo.Exists)
            directoryInfo.Delete(true);
        return directoryInfo;
    }
}
