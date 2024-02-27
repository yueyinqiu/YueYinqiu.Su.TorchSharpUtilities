using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YueYinqiu.Su.TorchSharpUtilities;

namespace Tests;
internal sealed record Configurations
{
    public int Seed { get; set; } = 1234;
    public string OutputPath { get; set; } =
        PathBuilder.MyDocuments
        .Join("MyTemporaryFiles")
        .Join("TorchSharpUtilitiesTests")
        .Join("Outputs");
    public string Wav1Path { get; set; } = "./wav1.wav";
    public string Wav2Path { get; set; } = "./wav2.wav";
}
