namespace YueYinqiu.Su.TorchSharpUtilities.Configurations;

public sealed class ConfigurationLoader<T> where T : IConfigurations, new()
{
    public ConfigurationLoader()
    {
        this.File = new FileInfo("./configuration.json");
        this.Default = new T();
        this.LoadingMessage =
            $"Loading configurations...{Console.Out.NewLine}";
        this.CreatingMessage =
            $"Cannot find the configuration file. " +
            $"This is not an error, " +
            $"and now the default one has been created.{Console.Out.NewLine}" +
            $"Please edit it as needed, " +
            $"and then restart the application: " +
            $"{this.File.FullName}";
        this.Terminator = () =>
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
        Console.WriteLine(this.LoadingMessage);

        if (this.File.Exists)
        {
            var result = HumanFriendlyJson.Deserialize<T>(this.File);
            if (result?.Version == this.Default.Version)
                return result;
        }

        HumanFriendlyJson.Serialize(this.File, this.Default);
        Console.WriteLine(this.CreatingMessage);

        this.Terminator();
        throw new Exception(this.CreatingMessage);
    }
}
