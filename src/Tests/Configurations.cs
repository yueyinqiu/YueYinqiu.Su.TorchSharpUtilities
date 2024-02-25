using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YueYinqiu.Su.TorchSharpUtilities;

namespace Tests;
internal class Configurations
{
    public int Seed { get; set; } = 1234;
    public string ModelPath { get; set; } =
        PathBuilder.MyDocuments
        .Join("MyMiscellaneousFiles")
        .Join("TorchSharpUtilitiesTests")
        .Join("TestModel");
}
