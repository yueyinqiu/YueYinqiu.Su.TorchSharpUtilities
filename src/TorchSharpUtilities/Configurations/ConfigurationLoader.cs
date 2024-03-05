namespace YueYinqiu.Su.TorchSharpUtilities.Configurations;

public sealed class ConfigurationLoader<T> where T : IConfigurations, new()
{
    public ConfigurationLoader()
    {
        File = new FileInfo("./configuration.json");
        Default = new T();
        LoadingMessage =
            $"Loading configurations...{Console.Out.NewLine}";
        CreatingMessage =
            $"Cannot find the configuration file. " +
            $"This is not an error, " +
            $"and now the default one has been created.{Console.Out.NewLine}" +
            $"Please edit it as needed, " +
            $"and then restart the application: " +
            $"{File.FullName}";
        Terminator = () =>
        {
            Console.Out.Flush();
            Environment.Exit(0);
        };
    }

    public FileInfo File { get; set; }
    public T Default { get; set; }
    public string LoadingMessage { get; set; }
    public string CreatingMessage { get; set; }
    public Action Terminator { get; set; }

    public T LoadOrCreate()
    {
        Console.WriteLine(LoadingMessage);

        if (File.Exists)
        {
            var result = HumanFriendlyJson.Deserialize<T>(File);
            if (result?.Version == Default.Version)
                return result;
        }

        HumanFriendlyJson.Serialize(File, Default);
        Console.WriteLine(CreatingMessage);

        Terminator();
        throw new Exception(CreatingMessage);
    }
}
