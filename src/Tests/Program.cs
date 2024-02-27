using NAudio.Wave;
using System.Text.Json;
using Tests;
using TorchSharp;
using YueYinqiu.Su.TorchSharpUtilities;
using YueYinqiu.Su.TorchSharpUtilities.Extensions;

Configurations configurations;

{
    Console.WriteLine("TEST 1:");
    configurations = new ConfigurationLoader<Configurations>().LoadOrCreate();
    Console.WriteLine(configurations);
    Console.WriteLine();
    Console.WriteLine();
}

{
    Console.WriteLine("TEST 2:");
    var output = Directory.CreateDirectory(configurations.OutputPath).CreatePathBuilder();

    using var reader1 = new AudioFileReader(configurations.Wav1Path);
    var audio1 = reader1.ReadAsTensor();
    Console.WriteLine(reader1.WaveFormat);
    Console.WriteLine(audio1.ToString(TensorStringStyle.CSharp));
    Console.WriteLine();

    using var reader2 = new AudioFileReader(configurations.Wav2Path);
    var audio2 = reader2.ReadAsTensor(false);
    Console.WriteLine(reader2.WaveFormat);
    Console.WriteLine(audio2.ToString(TensorStringStyle.CSharp));
    Console.WriteLine();

    using (var writer = new WaveFileWriter(output.Join("test1.wav"), new WaveFormat(16000, 1)))
    {
        writer.WriteTensor(audio1);
        writer.WriteTensor(audio2, false);
        Console.WriteLine($"The result has been written to {writer.Filename}");
    }

    using (var writer = new WaveFileWriter(output.Join("test2.wav"), new WaveFormat(20000, 2)))
    {
        var new2 = audio2.transpose(0, 1);
        var new1 = audio1[..(int)new2.size(0), ..(int)new2.size(1)];
        var newAudio = torch.concat([new1, new2], dim: 0);
        writer.WriteTensor(newAudio);

        newAudio = torch.concat([new2, new1], dim: 0);
        writer.WriteTensor(newAudio.transpose(0, 1), false);
        Console.WriteLine($"The result has been written to {writer.Filename}");
    }

    Console.WriteLine();
    Console.WriteLine();
}

{
    Console.WriteLine("TEST 3:");

    Console.WriteLine($"range(10): [{string.Join(", ", 
        PythonLike.Range(10))}]");
    Console.WriteLine($"range(0): [{string.Join(", ",
        PythonLike.Range(0))}]");
    Console.WriteLine($"range(-5): [{string.Join(", ",
        PythonLike.Range(-5))}]");
    Console.WriteLine();

    Console.WriteLine($"range(0, 10): [{string.Join(", ",
        PythonLike.Range(0, 10))}]");
    Console.WriteLine($"range(-10, 0): [{string.Join(", ", 
        PythonLike.Range(-10, 0))}]");
    Console.WriteLine();

    Console.WriteLine($"range(-10, 0, 1): [{string.Join(", ",
        PythonLike.Range(-10, 0, 1))}]");
    Console.WriteLine($"range(-10, 0, -1): [{string.Join(", ", 
        PythonLike.Range(-10, 0, -1))}]");
    Console.WriteLine();

    Console.WriteLine($"range(0, -10, -1): [{string.Join(", ", 
        PythonLike.Range(0, -10, -1))}]");
    Console.WriteLine($"range(0, -10, 1): [{string.Join(", ", 
        PythonLike.Range(0, -10, 1))}]");
    Console.WriteLine();

    Console.WriteLine($"range(1234567890, 1234567891): [{string.Join(", ",
        PythonLike.Range(1234567891234567890, 1234567891234567891))}]");
    Console.WriteLine();
    Console.WriteLine();
}

{
    Console.WriteLine("TEST 4:");

    var modules = PythonLike.Range(10).Select(_ => torch.nn.Linear(10, 10));
    var sequential = modules.ToSequential();
    var moduleList = modules.ToModuleList();

    Console.WriteLine(sequential.Count);
    Console.WriteLine(moduleList.Count);
}