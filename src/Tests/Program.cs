using NAudio.Wave;
using Python.Runtime;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Tests;
using Tests.Extensions;
using TorchSharp;
using YueYinqiu.Su.TorchSharpUtilities;
using YueYinqiu.Su.TorchSharpUtilities.Checkpoints;
using YueYinqiu.Su.TorchSharpUtilities.Configurations;
using YueYinqiu.Su.TorchSharpUtilities.Extensions;
using YueYinqiu.Su.TorchSharpUtilities.JsonSerialization;

Configurations configurations;

{
    Console.WriteLine("TEST 1:");
    configurations = new ConfigurationLoader<Configurations>().LoadOrCreate();
    Console.WriteLine(configurations);
    _ = Directory.CreateDirectory(configurations.OutputPath).Deleted();
    Console.WriteLine();
    Console.WriteLine();
}

{
    Console.WriteLine("TEST 2:");

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
    Console.WriteLine("TEST 3:");

    var modules = PythonLike.Range(10).Select(_ => torch.nn.Linear(10, 10));
    var sequential = modules.ToSequential();
    var moduleList = modules.ToModuleList();

    Console.WriteLine(sequential.Count);
    Console.WriteLine(moduleList.Count);

    var tensor = torch.rand([1, 10]);
    _ = tensor.print();
    Console.WriteLine(sequential.call(tensor).cstr());

    var outputDirectory = configurations.OutputPath / new PathBuilder("TEST 4");
    _ = outputDirectory.AsDirectory().Deleted();

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
    Console.WriteLine("TEST 4:");
    var output = Directory.CreateDirectory(configurations.OutputPath).CreatePathBuilder();
    output = output.Join("TEST 4").Join("try.xlsx");
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

    output.AsFile().DeleteDirectory();
    using var writer = new CsvWriter<Configurations>(output.AsFile());
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
    Console.WriteLine("TEST 5:");

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
    Console.WriteLine("TEST 6:");

    var path = Directory.CreateDirectory(configurations.OutputPath).CreatePathBuilder();
    path = path.Join("TEST 6").Join("test.json");

    HumanFriendlyJson.Serialize(path.AsFile(), "Hello World");
    Console.WriteLine(HumanFriendlyJson.Deserialize<string>(path.AsFile()));

    using var stream = new MemoryStream();
    HumanFriendlyJson.Serialize(stream, torch.zeros(3, 3));
    stream.Position = 0;
    Console.WriteLine(HumanFriendlyJson.Deserialize<torch.Tensor>(stream));

    var module = torch.nn.Linear(3, 3);
    Debug.Assert(module.bias is not null);
    module.bias = torch.nn.Parameter(torch.ones_like(module.bias) * 1234);
    var s = HumanFriendlyJson.Serialize(module.state_dict());
    var states = HumanFriendlyJson.Deserialize<Dictionary<string, torch.Tensor>>(s);
    module = torch.nn.Linear(3, 3);
    _ = module.load_state_dict(states);
    Console.WriteLine(module.bias?.cstr());

    var checkpoint = new TensorCheckpoint(torch.ones(3, 3) * 1234);
    checkpoint = HumanFriendlyJson.Clone(checkpoint);
    Console.WriteLine(checkpoint);
    Console.WriteLine();
    Console.WriteLine();
}

{
    Console.WriteLine("TEST 7:");

    var path = Directory.CreateDirectory(configurations.OutputPath).CreatePathBuilder();
    var directory = path.Join("TEST 7").AsDirectory().Deleted();

    var checkpointManager = new CheckpointManager<TensorCheckpoint>(directory);
    Console.WriteLine(checkpointManager.ListAll().Count());

    checkpointManager.Save(1234, new TensorCheckpoint(torch.zeros(3, 3) + 1234));
    checkpointManager.Save(2345, new TensorCheckpoint(torch.zeros(3, 3) + 2345));
    checkpointManager.Save(0123, new TensorCheckpoint(torch.zeros(3, 3) + 0123));
    Console.WriteLine(checkpointManager.Load());
    Console.WriteLine(checkpointManager.Load(1234));

    foreach (var checkpoint in checkpointManager.ListAll())
    {
        Console.WriteLine(checkpoint.index);
        Console.WriteLine(checkpoint.checkpoint.Value);
    }
    Console.WriteLine();
    Console.WriteLine();
}

{
    Console.WriteLine("TEST 1':");
    var output = Directory.CreateDirectory(configurations.OutputPath).CreatePathBuilder();
    output /= "TEST 1'";
    output.AsDirectory().Create();

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

    using (var writer = new WaveFileWriter(output.Join("test1.wav"), new WaveFormat(48000, 1)))
    {
        writer.WriteTensor(audio1);
        writer.WriteTensor(audio2, false);
        Console.WriteLine($"The result has been written to {writer.Filename}");
    }

    using (var writer = new WaveFileWriter(output.Join("test2.wav"), new WaveFormat(56000, 2)))
    {
        var new2 = audio2.transpose(0, 1)[..(int)audio1.size(0), ..(int)audio1.size(1)];
        var newAudio = torch.concat([audio1, new2], dim: 0);
        writer.WriteTensor(newAudio);

        newAudio = torch.concat([new2, audio1], dim: 0);
        writer.WriteTensor(newAudio.transpose(0, 1), false);
        Console.WriteLine($"The result has been written to {writer.Filename}");
    }

    using var reader3 = new AudioFileReader(output.Join("test2.wav"));
    using (var writer = new WaveFileWriter(output.Join("test3.wav"), new WaveFormat(48000, 2)))
    {
        var audio3 = reader3.ReadAsTensor();
        writer.WriteTensor(audio3.flip(0));
        Console.WriteLine($"The result has been written to {writer.Filename}");
    }

    Console.WriteLine();
    Console.WriteLine();
}

{
    Console.WriteLine("TEST 2':");

    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        Console.WriteLine("Skipped.");
    }
    else
    {
        var path = Directory.CreateDirectory(configurations.OutputPath).CreatePathBuilder();
        var directory = path.Join("TEST 2'").AsDirectory().Deleted();

        using var pythonZipStream = File.OpenRead(configurations.PythonPath);
        using var zip = new ZipArchive(pythonZipStream);
        zip.ExtractToDirectory(directory.FullName);

        var python = new EmbeddablePython(directory);
        for (int i = 0; i < 5; i++)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await python.EnsurePipAsync();
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }
        await python.PipInstallAsync("electrostaticvacuum");
        await python.PipInstallAsync("numpy==1.26.3");

        Console.WriteLine(python.InitializePythonNet());
        Console.WriteLine(python.InitializePythonNet());
        dynamic sys = Py.Import("sys");
        Console.WriteLine(sys.version);
        dynamic electrostaticvacuum = Py.Import("electrostaticvacuum");
        Console.WriteLine(electrostaticvacuum.__all__);
        dynamic numpy = Py.Import("numpy");
        Console.WriteLine(numpy.__version__);

        AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
        PythonEngine.Shutdown();
        AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", false);
    }

    Console.WriteLine();
    Console.WriteLine();
}
