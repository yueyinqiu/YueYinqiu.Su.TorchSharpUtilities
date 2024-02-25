using Tests;
using YueYinqiu.Su.TorchSharpUtilities;

var c = new ConfigurationLoader<Configurations>().LoadOrCreate();
Console.WriteLine(c.Seed);
Console.WriteLine(c.ModelPath);
