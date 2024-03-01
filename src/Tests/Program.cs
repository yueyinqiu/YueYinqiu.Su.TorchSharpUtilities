using NAudio.Wave;
using System.Diagnostics;
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

    var tensor = torch.rand([1, 10]);
    _ = tensor.print();
    Console.WriteLine(sequential.call(tensor).cstr());

    var outputDirectory = new PathBuilder(configurations.OutputPath).Join("no such dir");
    outputDirectory.AsDirectory().Create();
    outputDirectory.AsDirectory().Delete(true);

    var modelFile = outputDirectory.Join("model");
    _ = sequential.SaveWithDirectory(modelFile);
    var tensorDotnetFile = outputDirectory.Join("tensor dotnet");
    _ = tensor.SaveWithDirectory(tensorDotnetFile);
    var tensorFile = outputDirectory.Join("tensor");
    _ = tensor.SaveWithDirectory(tensorFile, false);

    var newSequential = modules.ToSequential();
    _ = newSequential.load(modelFile.AsFile().FullName);
    _ = sequential.call(torch.load(tensorFile)).print();
    var k = torch.Tensor.Load(tensorDotnetFile);
    _ = sequential.call(k).print();

    modelFile = outputDirectory.Join("model");
    sequential.SaveWithDirectory(modelFile.AsFile());
    tensorDotnetFile = outputDirectory.Join("tensor dotnet");
    tensor.SaveWithDirectory(tensorDotnetFile.AsFile());
    tensorFile = outputDirectory.Join("tensor");
    tensor.SaveWithDirectory(tensorFile.AsFile(), false);

    newSequential = modules.ToSequential();
    _ = newSequential.load(modelFile.AsFile().FullName);
    _ = sequential.call(torch.load(tensorFile)).print();
    k = torch.Tensor.Load(tensorDotnetFile);
    _ = sequential.call(k).print();

    Console.WriteLine();
    Console.WriteLine();
}

{
    Console.WriteLine("TEST 5:");
    var output = Directory.CreateDirectory(configurations.OutputPath).CreatePathBuilder();
    output = output.Join("try.xlsx");
    output = output.AsFile().CreatePathBuilder();
    output = output.ChangeExtension("csv");

    void CheckWritten()
    {
        Debug.Assert(output is not null);
        using FileStream fs = new FileStream(output,
            FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using StreamReader reader = new StreamReader(fs);
        Console.WriteLine($": {reader.ReadToEnd()}");
    }

    using var writer = new CsvWriter<Configurations>(output);
    CheckWritten();

    writer.Flush();
    CheckWritten();

    writer.Write(new Configurations());
    CheckWritten();

    writer.Flush();
    CheckWritten();

    writer.Write(new Configurations());
    CheckWritten();

    writer.Dispose();
    CheckWritten();
    Console.WriteLine();
    Console.WriteLine();
}

{
    Console.WriteLine("TEST 6:");

    var tensor = torch.rand(10);
    var arrayF = (float[])tensor.ToArray();
    Console.WriteLine(string.Join(' ', arrayF));
    Console.WriteLine();

    tensor = torch.rand(4, 2, torch.ScalarType.Float64) * 100;
    _ = tensor.print(style: TensorStringStyle.CSharp);
    Console.WriteLine();

    var arrayD = (int[,])tensor.ToArray(typeof(int));
    for (int i = 0; i < arrayD.GetLength(0); i++)
    {
        for (int j = 0; j < arrayD.GetLength(1); j++)
            Console.Write($"{arrayD[i, j]}({tensor[i, j].ToDouble()}) ");
        Console.WriteLine();
    }
    Console.WriteLine();
    Console.WriteLine();
}

{
    Console.WriteLine("TEST 7:");

    var path = Directory.CreateDirectory(configurations.OutputPath).CreatePathBuilder();
    path = path.Join("test.json");

    HumanFriendlyJson.Serialize(path.AsFile(), "Hello World");
    Console.WriteLine(HumanFriendlyJson.Deserialize<string>(path.AsFile()));

    using var stream = new MemoryStream();
    HumanFriendlyJson.Serialize(stream, "Hello World");
    stream.Position = 0;
    Console.WriteLine(HumanFriendlyJson.Deserialize<string>(stream));

    var s = HumanFriendlyJson.Serialize("Hello World");
    Console.WriteLine(HumanFriendlyJson.Deserialize<string>(s));
}