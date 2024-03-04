﻿using YueYinqiu.Su.TorchSharpUtilities;

namespace Tests;
internal sealed record Configurations : IConfigurations
{
    public int Seed { get; set; } = 1234;
    public string OutputPath { get; set; } =
        PathBuilder.MyDocuments
        .Join("MyTemporaryFiles")
        .Join("TorchSharpUtilitiesTests")
        .Join("Outputs");
    public string Wav1Path { get; set; } = "./wav1.flac";
    public string Wav2Path { get; set; } = "./wav2.flac";
    public string Version { get; set; } = "hello world";
}
